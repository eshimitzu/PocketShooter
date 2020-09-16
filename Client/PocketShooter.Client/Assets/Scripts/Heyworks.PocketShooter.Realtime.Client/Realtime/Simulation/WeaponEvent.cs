using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Simulation
{
#pragma warning disable SA1649 // File name should match first type name
    public struct WeaponStateChangedEvent : IClientEvent, IServerEvent, IActorEvent
#pragma warning restore SA1649 // File name should match first type name
    {
        public WeaponStateChangedEvent(EntityId actorId, WeaponState prevState, WeaponState nextState)
        {
            Next = nextState;
            Previous = prevState;
            ActorId = actorId;
        }

        public EntityId ActorId { get; }
        public WeaponState Previous { get; }
        public WeaponState Next { get; }
    }

    public struct WarmingUpProgressChangedEvent : IClientEvent, IServerEvent
    {
        public WarmingUpProgressChangedEvent(float progress)
        {
            Progress = progress;
        }

        public float Progress { get; }
    }

    public struct AmmoChangeEvent : IClientEvent, IServerEvent
    {
        public AmmoChangeEvent(int ammo)
        {
            Ammo = ammo;
        }

        public int Ammo { get; }
    }
}