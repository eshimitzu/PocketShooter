using System.Collections.Generic;
using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Entities
{
    public class PlayerEffects : IOwnedPlayerEffects, IRemotePlayerEffects
    {
        protected IRef<PlayerState> playerStateRef;

        public PlayerEffects(IRef<PlayerState> playerStateRef)
        {
            this.playerStateRef = playerStateRef;
        }

        public void ApplyState(IRef<PlayerState> playerStateRef)
        {
            this.playerStateRef = playerStateRef;
        }

        ref readonly StunBaseComponent IRemotePlayerEffects.Stun => ref Stun;

        ref readonly RootBaseComponent IRemotePlayerEffects.Root => ref Root;

        ref readonly InvisibleBaseComponent IRemotePlayerEffects.Invisible => ref Invisible;

        ref readonly ImmortalityBaseComponent IRemotePlayerEffects.Immortal => ref Immortal;

        ref readonly RageBaseComponent IRemotePlayerEffects.Rage => ref Rage;

        ref readonly JumpBaseComponent IRemotePlayerEffects.Jump => ref Jump;

        ref readonly LuckyBaseComponent IRemotePlayerEffects.Lucky => ref Lucky;

        ref readonly DashBaseComponent IRemotePlayerEffects.Dash => ref Dash;

        ref readonly MedKitBaseComponent IRemotePlayerEffects.MedKit => ref MedKit;

        ref readonly MedKitBaseComponent IRemotePlayerEffects.Heal => ref Heal;

        public ref StunBaseComponent Stun => ref playerStateRef.Value.Effects.Stun.Base;

        public ref RootBaseComponent Root => ref playerStateRef.Value.Effects.Root.Base;

        public ref InvisibleBaseComponent Invisible => ref playerStateRef.Value.Effects.Invisible.Base;

        public ref ImmortalityBaseComponent Immortal => ref playerStateRef.Value.Effects.Immortality.Base;

        public ref RageBaseComponent Rage => ref playerStateRef.Value.Effects.Rage.Base;

        public ref JumpBaseComponent Jump => ref playerStateRef.Value.Effects.Jump.Base;

        public ref LuckyBaseComponent Lucky => ref playerStateRef.Value.Effects.Lucky.Base;

        public ref DashBaseComponent Dash => ref playerStateRef.Value.Effects.Dash.Base;

        public ref MedKitBaseComponent MedKit => ref playerStateRef.Value.Effects.MedKit.Base;

        public ref MedKitBaseComponent Heal => ref playerStateRef.Value.Effects.Heal.Base;
    }
}