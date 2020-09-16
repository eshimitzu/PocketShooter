using System.Collections.Generic;
using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Entities
{
    /// <summary>
    /// Represents client player entity.
    /// </summary>
    public class ClientPlayer : OwnedPlayer, IClientPlayer
    {
        // may create and return readonly ref
        public IRef<PlayerState> Ref => playerStateRef;

        public ClientPlayer(IRef<PlayerState> playerStateRef, TrooperInfo trooperInfo)
            : base(playerStateRef, trooperInfo)
        {
            Events = new ClientPlayerEvents(Id);
        }

        // TODO: find out all properties which should be copied and what not
        public override void ApplyState(IRef<PlayerState> playerStateRef)
        {
            base.ApplyState(playerStateRef);

            // NOTE: we use local state for fast feedback, but should check that server state is not so different from ours, i.e. reconcile
            // Reconciliation requires next ingredients:
            // state for last server confirmed tick
            // state we predicted for that tick
            // if state is different
            //   took all commands from that tick from history
            //   replay all commands locally
            //   if replay leads to change of state in compare to history - raise event
            //   settle on last state
            var transformComponent = playerStateRef.Value.Transform;
            var weaponComponents = playerStateRef.Value.Weapon;

            CurrentWeapon.ApplyState(playerStateRef);

            playerStateRef.Value.Transform = transformComponent;
            playerStateRef.Value.Weapon = weaponComponents;
        }

        IClientPlayerEvents IVisualPlayer<IClientPlayerEvents>.Events => Events;

        public IReadOnlyList<ShotInfo> Shots => playerStateRef.Value.Shots;

        public IReadOnlyList<DamageInfo> Damages => playerStateRef.Value.Damages;

        public new IReadOnlyList<HealInfo> Heals => playerStateRef.Value.Heals;

        public ClientPlayerEvents Events { get; }
    }
}