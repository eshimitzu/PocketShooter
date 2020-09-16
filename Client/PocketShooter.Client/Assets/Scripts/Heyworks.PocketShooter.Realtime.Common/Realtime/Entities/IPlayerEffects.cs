using System.Collections.Generic;
using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Entities
{
    public interface IPlayerEffects
    {
        void ApplyState(IRef<PlayerState> playerStateRef);
    }

    public interface IRemotePlayerEffects : IPlayerEffects
    {
        ref readonly StunBaseComponent Stun { get; }

        ref readonly RootBaseComponent Root { get; }

        ref readonly InvisibleBaseComponent Invisible { get; }

        ref readonly ImmortalityBaseComponent Immortal { get; }

        ref readonly RageBaseComponent Rage { get; }

        ref readonly JumpBaseComponent Jump { get; }

        ref readonly LuckyBaseComponent Lucky { get; }

        ref readonly DashBaseComponent Dash { get; }

        ref readonly MedKitBaseComponent MedKit { get; }

        ref readonly MedKitBaseComponent Heal { get; }
    }

    public interface IOwnedPlayerEffects : IRemotePlayerEffects, IPlayerEffects
    {
        new ref StunBaseComponent Stun { get; }

        new ref RootBaseComponent Root { get; }

        new ref InvisibleBaseComponent Invisible { get; }

        new ref ImmortalityBaseComponent Immortal { get; }

        new ref RageBaseComponent Rage { get; }

        new ref JumpBaseComponent Jump { get; }

        new ref LuckyBaseComponent Lucky { get; }

        new ref DashBaseComponent Dash { get; }

        new ref MedKitBaseComponent MedKit { get; }

        new ref MedKitBaseComponent Heal { get; }
    }
}