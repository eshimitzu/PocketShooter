using System.Collections.Generic;
using Collections.Pooled;
using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Entities
{
    public abstract class PlayerBase : IPlayer
    {
        protected IRef<PlayerState> playerStateRef;

        protected PlayerBase(IRef<PlayerState> playerStateRef)
        {
            this.playerStateRef = playerStateRef;
            this.Effects = new PlayerEffects(playerStateRef);
        }

        public virtual void ApplyState(IRef<PlayerState> playerStateRef)
        {
            this.playerStateRef = playerStateRef;
            this.Effects.ApplyState(playerStateRef);
        }

        internal IRef<PlayerState> StateRef => playerStateRef;

        public EntityId Id => playerStateRef.Value.Id;

        public TrooperClass TrooperClass => playerStateRef.Value.TrooperClass;

        public string Nickname => playerStateRef.Value.Nickname;

        public TeamNo Team => playerStateRef.Value.Team;

        IRemotePlayerEffects IPlayer.Effects => Effects;

        public bool IsAlive => Health.Health > 0;

        public bool IsDead => !IsAlive;

        public bool IsStunned => Effects.Stun.IsStunned;

        public bool IsRooted => Effects.Root.IsRooted;

        public ref FpsTransformComponent Transform => ref playerStateRef.Value.Transform;

        ref readonly HealthComponent IPlayer.Health => ref Health;

        ref readonly ArmorComponent IPlayer.Armor => ref Armor;

        ref readonly FpsTransformComponent IPlayer.Transform => ref Transform;

        ref readonly RemoteServerEvents IPlayer.ServerEvents => ref ServerEvents;

        IReadOnlyList<HealInfo> IPlayer.Heals => Heals;

        internal IOwnedPlayerEffects Effects { get; }

        internal ref HealthComponent Health => ref playerStateRef.Value.Health;

        internal ref ArmorComponent Armor => ref playerStateRef.Value.Armor;

        internal ref RemoteServerEvents ServerEvents => ref playerStateRef.Value.ServerEvents;

        internal PooledList<HealInfo> Heals => playerStateRef.Value.Heals;
    }
}