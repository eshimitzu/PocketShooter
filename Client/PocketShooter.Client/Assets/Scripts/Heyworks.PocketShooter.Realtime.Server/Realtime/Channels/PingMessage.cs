using Collections.Pooled;
using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Channels
{
    internal sealed class PingMessage : IMessage
    {
        public PingMessage(PlayerId playerId, SimulationMetaCommandData data, PooledList<SimulationCommandData> commands)
        {
            PlayerId = playerId;
            Ping = data;
            Commands = commands;
        }

        public PlayerId PlayerId { get; }

        public SimulationMetaCommandData Ping { get; }

        public PooledList<SimulationCommandData> Commands { get; }
    }
}