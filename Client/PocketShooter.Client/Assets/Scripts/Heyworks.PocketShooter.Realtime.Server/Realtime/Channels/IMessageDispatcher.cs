using Heyworks.PocketShooter.Realtime.MessageProcessors;
using Heyworks.PocketShooter.Realtime.Channels;

namespace Heyworks.PocketShooter.Realtime
{
    public interface IMessageDispatcher
    {
        void AddMessageProcessor(IMessageProcessor processor);

        void RemoveMessageProcessor(IMessageProcessor processor);

        IMessageProcessor GetMessageProcessor(IMessage message);
    }
}
