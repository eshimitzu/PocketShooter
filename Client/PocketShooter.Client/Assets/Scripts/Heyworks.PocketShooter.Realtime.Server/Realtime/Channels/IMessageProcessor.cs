using Heyworks.PocketShooter.Realtime.Channels;

namespace Heyworks.PocketShooter.Realtime.MessageProcessors
{
    public interface IMessageProcessor
    {
        bool CanProcessMessage(IMessage message);

        void ProcessMessage(IMessage message);
    }
}
