using System.Threading;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using System.Threading.Tasks;
using Heyworks.PocketShooter.Realtime.Channels;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.MessageProcessors;
using Microsoft.Extensions.Logging;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;

namespace Heyworks.PocketShooter.Realtime
{
    public class ClientPeerEx : ClientPeer, IPeer
    {
        private readonly ClientStateMachine stateMachine;
        private readonly CancellationTokenSource disconnectCancellationSource;
        private readonly ILogger logger;
        private readonly Channel<IMessage> peerChannel;

        public ClientPeerEx(InitRequest initRequest, IClientState initState, Channel<IMessage> peerChannel, ILoggerFactory loggerFactory)
            : base(initRequest)
        {
            this.logger = loggerFactory.CreateLogger(nameof(ClientPeerEx));
            this.peerChannel = peerChannel;
            this.disconnectCancellationSource = new CancellationTokenSource();
            Task.Run(() => ReadMessages(peerChannel, disconnectCancellationSource.Token));
            this.stateMachine = new ClientStateMachine(initState);
        }

        public void AddStateTransition(string fromStateName, Type onMessageType, IClientStateFactory useStateFactory) =>
            stateMachine.AddTransition(fromStateName, onMessageType, useStateFactory);

        /// <summary>
        /// Enqueues OnDisconnectByOtherPeer to the RequestFiber.
        /// This method is intended to be used to disconnect a user's peer if he connects with multiple clients
        /// while the application logic wants to allow just one.
        /// </summary>
        /// <param name="otherPeer">The other peer.</param>
        /// <param name="otherRequest">The other request.</param>
        /// <param name="sendParameters">The send Parameters.</param>
        public void DisconnectByOtherPeer(PeerBase otherPeer, OperationRequest otherRequest, SendParameters sendParameters) =>
            RequestFiber.Enqueue(() => OnDisconnectByOtherPeer(otherPeer, otherRequest, sendParameters));

        /// <summary>
        /// Converts object to its string representation.
        /// </summary>
        public override string ToString() =>
            $"Remote address: {RemoteIP}:{RemotePort}. Connection Id: {ConnectionId}";

        /// <summary>
        /// Executed when a peer disconnects.
        /// </summary>
        /// <param name="reasonCode">The disconnect reason.</param>
        /// <param name="reasonDetail">The details about disconnect reason.</param>
        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            logger.LogInformation(
                "UDP peer {peer} disconnected. Reason {reasonCode}, {reasonDetail}",
                this,
                reasonCode,
                reasonDetail);

            disconnectCancellationSource.Cancel();
            peerChannel.Writer.Complete();
            CurrentState.HandleDisconnect();
        }

        /// <summary>
        /// Incoming OperationRequests are handled here.
        /// </summary>
        /// <param name="operationRequest">The operation Request.</param>
        /// <param name="sendParameters">The send Parameters.</param>
        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters) =>
            CurrentState.HandleData(operationRequest.OperationCode, (byte[])operationRequest.Parameters[0]);

        /// <summary>
        /// Gets the current client state.
        /// </summary>
        protected IClientState CurrentState => stateMachine.CurrentState;

        /// <summary>
        /// Called by DisconnectByOtherPeer after being enqueued to the RequestFiber.
        /// It calls CurrentState.HandleDisconnectByOtherPeer and then continues the otherRequest by calling the otherPeer.OnOperationRequest method.
        /// </summary>
        /// <param name="otherPeer">The original peer.</param>
        /// <param name="otherRequest">The original request.</param>
        /// <param name="sendParameters">The send Parameters.</param>
        protected virtual void OnDisconnectByOtherPeer(PeerBase otherPeer, OperationRequest otherRequest, SendParameters sendParameters)
        {
            logger.LogInformation("Other client peer {otherPeer} disconnect this peer {peer}", otherPeer, this);
            RequestFiber.Enqueue(Disconnect);
            PeerHelper.InvokeOnOperationRequest(otherPeer, otherRequest, sendParameters);
        }

        private async Task ReadMessages(ChannelReader<IMessage> reader, CancellationToken disconnected)
        {
            while (!disconnectCancellationSource.IsCancellationRequested && await reader.WaitToReadAsync(disconnected))
            {
                while (!disconnectCancellationSource.IsCancellationRequested && reader.TryRead(out var message))
                {
                    try
                    {
                        ProcessMessage(message);
                    }
                    catch (Exception ex)
                    {
                        SimulationLog.Log.LogCritical(ex, "Unhandled exception during game simulation.");
                        var data = new[] { (byte)ClientErrorCode.SomethingWrongHappenedAndWeDoNotKnowWhat };
                        SendEvent(NetworkDataCode.ServerError, data, false);
                    }
                }
            }

            logger.LogInformation("Player from {RemoteIP}:{RemotePort} on {ConnectionId} connection stopped to process messages", RemoteIP, RemotePort, ConnectionId);
        }

        private void ProcessMessage(IMessage message)
        {
            CurrentState.ProcessMessage(message);
            stateMachine.OnMessage(message);
        }

        // we are in single fiber-thread
        private readonly Dictionary<byte, object> dataContainer = new Dictionary<byte, object>(1);

        public void SendEvent(NetworkDataCode dataCode, byte[] data, SendOptions sendOptions)
        {
            dataContainer.Add(0, data);
            ref var wrapper = ref Unsafe.As<SendOptions, SendParameters>(ref sendOptions);
            SendEvent(new EventData((byte)dataCode, dataContainer), wrapper);
            dataContainer.Clear();
        }

        public void SendEvent(NetworkDataCode dataCode, byte[] data, bool unreliable) =>
            SendEvent(dataCode, data, new SendOptions { Unreliable = unreliable });
    }
}
