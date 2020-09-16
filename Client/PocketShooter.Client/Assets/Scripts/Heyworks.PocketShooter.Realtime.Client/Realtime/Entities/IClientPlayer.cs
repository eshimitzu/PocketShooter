using System.Collections.Generic;
using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Entities
{
    /// <summary>
    /// Represents readonly interface for owned readonly player entities.
    /// </summary>
    public interface IClientPlayer : IOwnedPlayer, IVisualPlayer<IClientPlayerEvents>
    {
        IReadOnlyList<ShotInfo> Shots { get; }

        IReadOnlyList<DamageInfo> Damages { get; }
    }
}