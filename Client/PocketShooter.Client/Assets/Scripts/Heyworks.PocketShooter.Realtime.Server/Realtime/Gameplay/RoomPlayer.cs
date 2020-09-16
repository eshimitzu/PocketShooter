using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using Heyworks.PocketShooter.Realtime.Channels;
using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Gameplay
{
    internal sealed class RoomPlayer
    {
        private readonly ChannelWriter<IMessage> channel;

        private int? lastCommandTick;

        public RoomPlayer(PlayerInfo playerInfo, TeamNo teamNo, EntityId trooperId, ChannelWriter<IMessage> channel)
        {
            Info = playerInfo;
            TeamNo = teamNo;
            TrooperId = trooperId;
            this.channel = channel;
            ControlledBots = new HashSet<BotId>();
            ConfirmedTick = -1;
        }

        public PlayerId Id => Info.Id;

        public PlayerInfo Info { get; }

        public TeamNo TeamNo { get; }

        public EntityId TrooperId { get; }

        public int ConfirmedTick { get; private set; }

        public ISet<BotId> ControlledBots { get; }

        public ValueTask SendMessage(IMessage message) => channel.WriteAsync(message);

        /// <summary>
        /// Registers the last command tick.
        /// </summary>
        /// <param name="clientTick">The client simulation tick.</param>
        public void RegisterLastCommandTick(int clientTick)
        {
            // if no value or there is value that less that last known, overwrite it
            if (!lastCommandTick.HasValue || lastCommandTick.Value < clientTick)
            {
                lastCommandTick = clientTick;
            }
        }

        /// <summary>
        /// Gets the last command tick delay by player Id.
        /// </summary>
        /// <param name="currentTick">The current simulation tick.</param>
        public int GetLastCommandTickDelay(int currentTick)
        {
            return lastCommandTick.HasValue ? lastCommandTick.Value - currentTick : 1;
        }

        public void ConfirmTick(int tick) => ConfirmedTick = tick;

        public TrooperInfo GetTrooperInfo(TrooperClass trooperClass) =>
            Info.Troopers.First(item => item.Class == trooperClass);
    }
}
