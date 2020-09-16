using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public sealed class MoveSystem : IOwnerSystem
    {
        private readonly MoveCommandData commandData;

        public MoveSystem(MoveCommandData commandData)
        {
            this.commandData = commandData;
        }

        public bool Execute(OwnedPlayer initiator)
        {
            if (initiator.IsAlive &&
                !initiator.Effects.Stun.IsStunned &&
                !initiator.Effects.Root.IsRooted)
            {
                ref var t = ref initiator.Transform;
                ref readonly var moved = ref commandData.Transform;
                t.Position = moved.Position;
                t.Yaw = moved.Yaw;
                t.Pitch = moved.Pitch;

                return true;
            }

            return false;
        }
    }
}