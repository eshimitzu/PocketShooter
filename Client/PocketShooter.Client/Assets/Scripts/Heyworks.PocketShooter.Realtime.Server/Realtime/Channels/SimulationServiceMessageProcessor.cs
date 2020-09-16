using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Channels;
using Heyworks.PocketShooter.Realtime.Serialization;
using Microsoft.Extensions.Logging;

namespace Heyworks.PocketShooter.Realtime.MessageProcessors
{
    internal sealed class SimulationServiceMessageProcessor : IMessageProcessor
    {
        private static readonly IDataSerializer<BotControlTakenData> botControlTakenSerializer = new BotControlTakenDataSerializer();
        private static readonly IDataSerializer<GameEndedData> gameEndedSerializer = new GameEndedDataSerializer();

        private readonly IPeer peer;
        private readonly ILogger logger;

        public SimulationServiceMessageProcessor(IPeer peer, ILogger logger)
        {
            this.peer = peer;
            this.logger = logger;
        }

        public bool CanProcessMessage(IMessage message) =>
            message is BotControlTakenMessage ||
            message is GameEndedMessage ||
            message is ServerErrorMessage;

        public void ProcessMessage(IMessage message)
        {
            switch (message)
            {
                case BotControlTakenMessage bctm:
                    var botControlTaken = new BotControlTakenData(bctm.BotInfo, bctm.TeamNo, bctm.TrooperId);
                    peer.SendEvent(NetworkDataCode.BotControlTaken, botControlTakenSerializer.Serialize(botControlTaken), false);
                    break;
                case GameEndedMessage gem:
                    var gameEnded = new GameEndedData(gem.Tick);
                    peer.SendEvent(NetworkDataCode.GameEnded, gameEndedSerializer.Serialize(gameEnded), false);
                    break;
                case ServerErrorMessage error:
                    {
                        switch (error.Case)
                        {
                            case ServerErrorKind.Client when error.Client is ClientException ex:
                                logger.LogWarning(ex, "Will send error to client");
                                peer.SendEvent(NetworkDataCode.ServerError, new byte[] { (byte)ex.ErrorCode }, false);
                                break;

                            case ServerErrorKind.Server when error.Server is ServerException ex:
                                peer.SendEvent(NetworkDataCode.ServerError, new byte[] { (byte)ex.ErrorCode }, false);
                                break;
                            default:
                                error.Default();
                                break;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
