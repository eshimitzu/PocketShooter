using System.Collections.Generic;
using System.Linq;
using Heyworks.PocketShooter.Realtime.MessageProcessors;
using Heyworks.PocketShooter.Realtime.Channels;

namespace Heyworks.PocketShooter.Realtime
{
    public class MessageDispatcher : IMessageDispatcher
    {
        private readonly IList<IMessageProcessor> messageProcessors;

        public MessageDispatcher() =>
            messageProcessors = new List<IMessageProcessor>();

        public void AddMessageProcessor(IMessageProcessor processor) =>
            messageProcessors.Add(processor);

        public IMessageProcessor GetMessageProcessor(IMessage message) =>
            messageProcessors.FirstOrDefault(item => item.CanProcessMessage(message));

        public void RemoveMessageProcessor(IMessageProcessor processor) =>
            messageProcessors.Remove(processor);
    }
}
