using Heyworks.PocketShooter.Realtime.Channels;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Serialization;
using Heyworks.Realtime.Serialization;
using Microsoft.Extensions.Logging;

namespace Heyworks.PocketShooter.Realtime.MessageProcessors
{
    internal sealed class SimulationStateMessageProcessor : IMessageProcessor
    {
        private readonly IDeltaDataSerializer<SimulationState> serializer = new SimulationStateSerializer(Constants.BufferSize);
        private readonly IPeer peer;
        private readonly ILogger logger;

        public SimulationStateMessageProcessor(IPeer peer, ILogger logger)
        {
            this.peer = peer;
            this.logger = logger;
        }

        public bool CanProcessMessage(IMessage message) => message is SimulationStateMessage;

        public void ProcessMessage(IMessage message)
        {
            var msg = (SimulationStateMessage)message;
            logger.LogTrace("Device {connectionId} state sent baselined on {baselineTick} with {worldTick} server tick", peer.ConnectionId, msg.Baseline != null ? msg.Baseline.Value.Tick.ToString() : "'dummy state'", msg.Updated.Tick);

            var data = serializer.Serialize(in msg.Baseline, in msg.Updated);
            peer.SendEvent(NetworkDataCode.SimulationState, data, true);
        }
    }
}
