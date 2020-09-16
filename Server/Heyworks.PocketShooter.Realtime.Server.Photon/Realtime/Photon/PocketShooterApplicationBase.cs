using System;
using System.Threading.Channels;
using Heyworks.PocketShooter.Realtime.Channels;
using Heyworks.PocketShooter.Realtime.Photon.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Photon.SocketServer;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

namespace Heyworks.PocketShooter.Realtime.Server
{
    /// <summary>
    /// The Pocket Shooter Photon application.
    /// </summary>
    public class PocketShooterApplicationBase : ApplicationBase
    {
        protected IHost host;

        /// <summary>
        ///  Creates Pocket Shooter peer when connection received.
        /// </summary>
        /// <param name="initRequest">The initialization request.</param>
        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            // sometimes CreatePeer is called earlier than Setup, so we ensure setup was done here
            Setup();

            var services = host.Services;
            var loggerFactory = services.GetService<ILoggerFactory>();

            var logger = loggerFactory.CreateLogger(nameof(InitRequest));

            logger.LogDebug(
                "Connection with id {ConnectionId} received from {RemoteIP}",
                initRequest.ConnectionId,
                initRequest.RemoteIP);

            var peerChannel = Channel.CreateUnbounded<IMessage>(
                new UnboundedChannelOptions
                {
                    SingleReader = true,
                    SingleWriter = false,
                });

            var peer = new ClientPeerEx(initRequest, new ClientUnauthorizedState(), peerChannel, loggerFactory);

            var gameManagementChannel = services.GetService<IGameManagementChannel>();

            peer.AddStateTransition(
                StateNames.ClientUnauthorized,
                typeof(LobbyEnteredMessage),
                new ClientInLobbyStateFactory(
                    peer,
                    gameManagementChannel,
                    peerChannel,
                    loggerFactory));

            peer.AddStateTransition(
                StateNames.ClientInLobby,
                typeof(GameJoinedMessage),
                new ClientInGameStateFactory(peer, gameManagementChannel, peerChannel, loggerFactory));

            peerChannel.Writer.WriteAsync(new LobbyEnteredMessage());
            return peer;
        }

        protected override void Setup()
        {
            // does nothing for now, should share Development and Deploy items
        }

        protected void SetupStatic(IHost host)
        {
            var loggerFactory = host.Services.GetService<ILoggerFactory>();
            ExitGames.Logging.LogManager.SetLoggerFactory(new ExitGamesLoggerFactory(loggerFactory));
            MLog.Setup(loggerFactory);
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            AppDomain.CurrentDomain.ProcessExit -= CurrentDomain_ProcessExit;
        }

        /// <inheritdoc />
        protected override void TearDown() => host?.Dispose();
    }
}
