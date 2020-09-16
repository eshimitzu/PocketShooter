using Heyworks.PocketShooter.Realtime.Channels;
using Microsoft.Extensions.Logging;

namespace Heyworks.PocketShooter.Realtime
{
    /// <summary>
    /// Represents abstract class for the client state.
    /// </summary>
    public abstract class ClientState : IClientState
    {
        private readonly ILogger logger;
        private readonly IDataDispatcher dataDispatcher;
        private readonly IMessageDispatcher messageDispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientState"/> class.
        /// </summary>
        protected ClientState(IDataDispatcher dataDispatcher, IMessageDispatcher messageDispatcher, ILogger logger)
        {
            this.logger = logger;
            this.dataDispatcher = dataDispatcher;
            this.messageDispatcher = messageDispatcher;
        }

        /// <summary>
        /// Gets the state name.
        /// </summary>
        public abstract string Name
        {
            get;
        }

        public void HandleData(byte dataCode, byte[] data)
        {
            var dataHandler = dataDispatcher.GetDataHandler(dataCode);

            if (dataHandler != null)
            {
                dataHandler.HandleData(dataCode, data);
            }
            else
            {
                logger.LogWarning("State {Name} can't handle data code {DataCode}", Name, dataCode);
            }
        }

        public void ProcessMessage(IMessage message)
        {
            var messageProcessor = messageDispatcher.GetMessageProcessor(message);

            if (messageProcessor != null)
            {
                messageProcessor.ProcessMessage(message);
            }
            else
            {
                logger.LogWarning("State {Name} can't process message of type {MessageType}", Name, message.GetType().Name);
            }
        }

        /// <summary>
        /// Method responsible for handling client disconnects.
        /// </summary>
        public virtual void HandleDisconnect()
        {
        }
    }
}
