using System;
using System.Collections.Generic;
using System.Net;
using ExitGames.Client.Photon;
using Heyworks.PocketShooter.Realtime.Data;
using Microsoft.Extensions.Logging;

namespace Heyworks.PocketShooter.Realtime.Connection
{
    public class PhotonConnection : IPhotonPeerListener, IGameplayConnection
    {
        public static IGameplayConnection CreateDefault(IPEndPoint serverAddress)
        {
            var photonConfig = new PhotonConnectionConfiguration
            {
                Name = "Test",
                ServerAddress = serverAddress.ToString(),
                ApplicationName = "PocketShooter",
                EstablishEncryption = true,
            };

            return new PhotonConnection(photonConfig);
        }

        private PhotonPeer photonPeer;
        private PhotonConnectionConfiguration photonConfiguration;
        private Dictionary<byte, object> dataContainer = new Dictionary<byte, object>(1);
        private Queue<NetworkData> dataQueue;

        public event Action OnDisconnected;

        public event Action OnDisconnectedByServer;

        public PhotonConnection(PhotonConnectionConfiguration photonConfiguration)
        {
            this.photonConfiguration = photonConfiguration;

            photonPeer = new PhotonPeer(this, ConnectionProtocol.Udp);
            dataQueue = new Queue<NetworkData>();
        }

        public ConnectionState ConnectionState { get; private set; }

        public void Connect()
        {
            if (ConnectionState == ConnectionState.Disconnected)
            {
                var connectionState = photonPeer.Connect(
                    photonConfiguration.ServerAddress,
                    photonConfiguration.ApplicationName);
                if (connectionState)
                {
                    ConnectionState = ConnectionState.Connecting;
                }
            }
        }

        public void Disconnect() => photonPeer.Disconnect();

        public string ServerAddress => photonConfiguration.ServerAddress;

        public int RoundTripTimeMs => photonPeer.RoundTripTime;

        public int LastRoundTripTimeMs => GetLastRoundTripTime();

        public int RoundTripTimeVariance => photonPeer.RoundTripTimeVariance;

        public int ServerTimeMs => photonPeer.ServerTimeInMilliSeconds;

        public void QueueData(NetworkData data, bool reliable)
        {
            dataContainer.Add(0, data.Data);
            photonPeer.OpCustom((byte)data.Code, dataContainer, reliable);
            dataContainer.Clear();
        }

        public void Send()
        {
            do
            {
            }
            while (photonPeer.SendOutgoingCommands());
        }

        public void Receive()
        {
            do
            {
            }
            while (photonPeer.DispatchIncomingCommands());
        }

        public bool HasData() => dataQueue.Count > 0;

        public NetworkData GetData() => dataQueue.Dequeue();

        /// <summary>
        /// Provides textual descriptions for various error conditions and noteworthy situations.
        /// In cases where the application needs to react, a call to OnStatusChanged is used.
        /// OnStatusChanged gives "feedback" to the game, DebugReturn provides human readable messages
        /// on the background.
        /// </summary>
        /// <param name="level">DebugLevel (severity) of the message.</param><param name="message">Debug text. Print to System.Console or screen.</param>
        void IPhotonPeerListener.DebugReturn(DebugLevel level, string message)
        {
        }

        void IPhotonPeerListener.OnOperationResponse(OperationResponse operationResponse)
        {
        }

        /// <summary>
        /// OnStatusChanged is called to let the game know when asynchronous actions finished or when errors happen.
        /// </summary>
        /// <param name="statusCode">A code to identify the situation.</param>
        void IPhotonPeerListener.OnStatusChanged(StatusCode statusCode)
        {
            switch (statusCode)
            {
                case StatusCode.Disconnect:
                    OnDisconnected?.Invoke();
                    break;
                case StatusCode.DisconnectByServer:
                case StatusCode.DisconnectByServerLogic:
                case StatusCode.DisconnectByServerUserLimit:
                case StatusCode.TimeoutDisconnect:
                    OnDisconnectedByServer?.Invoke();
                    break;
            }

            switch (statusCode)
            {
                case StatusCode.Connect:
                    NetLog.Log.OnRealtimeServerConnected(statusCode);
                    OnConnected();
                    break;
                case StatusCode.EncryptionEstablished:
                    NetLog.Log.OnRealtimeConnectionStatusChanged(statusCode);
                    OnEncryptionEstablished();
                    break;
                case StatusCode.EncryptionFailedToEstablish:
                    NetLog.Log.LogError("Failed to establish encryption");
                    OnEncryptionFailedToEstablish();
                    break;
                case StatusCode.Disconnect:
                case StatusCode.DisconnectByServer:
                case StatusCode.DisconnectByServerLogic:
                case StatusCode.DisconnectByServerUserLimit:
                case StatusCode.TimeoutDisconnect:
                    NetLog.Log.OnRealtimeServerDisconnected(statusCode);
                    Disconnected();
                    break;
                default:
                    NetLog.Log.OnRealtimeConnectionStatusChanged(statusCode);
                    break;
            }
        }

        /// <summary>
        /// Called whenever an event from the Photon Server is dispatched.
        /// </summary>
        /// <param name="eventData">The event currently being dispatched.</param>
        public void OnEvent(EventData eventData)
        {
            var data = new NetworkData();
            data.Code = (NetworkDataCode)eventData.Code;
            data.Data = (byte[])eventData.Parameters[0];
            dataQueue.Enqueue(data);
        }

        private void OnConnected()
        {
            if (photonConfiguration.EstablishEncryption)
            {
                photonPeer.EstablishEncryption();
            }
            else
            {
                Connected();
            }
        }

        private void OnEncryptionEstablished() => Connected();

        private void OnEncryptionFailedToEstablish() => Disconnected();

        private void Connected() => ConnectionState = ConnectionState.Connected;

        private void Disconnected() => ConnectionState = ConnectionState.Disconnected;

        // TODO: a.dezhurko Ask ExitGames to make lastRoundTripTime public.
        private int GetLastRoundTripTime()
        {
            var peerBase = FieldInfoValueProvider<PhotonPeer>.GetFieldValue<PeerBase>(photonPeer, "peerBase");

            return peerBase != null ? FieldInfoValueProvider<PeerBase>.GetFieldValue<int>(peerBase, "lastRoundTripTime") : RoundTripTimeMs;
        }
    }
}