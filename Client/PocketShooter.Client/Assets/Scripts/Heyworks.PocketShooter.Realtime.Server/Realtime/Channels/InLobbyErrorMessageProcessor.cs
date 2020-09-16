using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.MessageProcessors;

namespace Heyworks.PocketShooter.Realtime.Channels
{
    internal sealed class InLobbyErrorMessageProcessor : IMessageProcessor
    {
        private readonly IPeer peer;

        public InLobbyErrorMessageProcessor(IPeer peer)
        {
            this.peer = peer;
        }

        public bool CanProcessMessage(IMessage message) =>
            message is ServerErrorMessage;

        public void ProcessMessage(IMessage message)
        {
            switch (message)
            {
                case ServerErrorMessage error:
                    {
                        switch (error.Case)
                        {
                            case ServerErrorKind.Client when error.Client is ClientException ex:
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
