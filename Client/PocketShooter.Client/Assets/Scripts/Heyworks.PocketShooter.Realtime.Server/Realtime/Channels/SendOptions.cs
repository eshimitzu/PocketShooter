﻿namespace Heyworks.PocketShooter.Realtime.MessageProcessors
{
    /// <summary>
    ///   The struct contains the parameters for <see cref="M:Photon.SocketServer.PeerBase.SendOperationResponse(Photon.SocketServer.OperationResponse,Photon.SocketServer.SendParameters)" />, <see cref="M:Photon.SocketServer.PeerBase.SendEvent(Photon.SocketServer.IEventData,Photon.SocketServer.SendParameters)" /> and <see cref="M:Photon.SocketServer.ServerToServer.S2SPeerBase.SendOperationRequest(Photon.SocketServer.OperationRequest,Photon.SocketServer.SendParameters)" />
    ///   and contains the info about incoming data at <see cref="M:Photon.SocketServer.PeerBase.OnOperationRequest(Photon.SocketServer.OperationRequest,Photon.SocketServer.SendParameters)" />, <see cref="M:Photon.SocketServer.ServerToServer.S2SPeerBase.OnEvent(Photon.SocketServer.IEventData,Photon.SocketServer.SendParameters)" /> and <see cref="M:Photon.SocketServer.ServerToServer.S2SPeerBase.OnOperationResponse(Photon.SocketServer.OperationResponse,Photon.SocketServer.SendParameters)" />.
    /// </summary>
    public struct SendOptions
    {
        /// <summary>
        ///   Gets or sets the channel id for the udp protocol.
        /// </summary>
        public byte ChannelId
        {
            get;
            set;
        }

        /// <summary>
        ///   Gets or sets a value indicating whether the data is sent encrypted.
        /// </summary>
        public bool Encrypted
        {
            get;
            set;
        }

        /// <summary>
        ///   Gets or sets a value indicating whether to flush all queued data with the next send.
        ///   This overrides the configured send delay.
        /// </summary>
        public bool Flush
        {
            get;
            set;
        }

        /// <summary>
        ///   Gets or sets a value indicating whether to send the data unreliable.
        /// </summary>
        public bool Unreliable
        {
            get;
            set;
        }
    }
}
