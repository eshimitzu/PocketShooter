using System.Collections.Generic;
using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Entities
{
    /// <summary>
    /// Represents readonly interface above networked players.
    /// </summary>
    public interface IPlayer : IPlayerEntity
    {
        string Nickname { get; }

        /// <summary>
        /// Gets the player team.
        /// </summary>
        TeamNo Team { get; }

        TrooperClass TrooperClass { get; }

        ref readonly HealthComponent Health { get; }

        ref readonly ArmorComponent Armor { get; }

        IRemotePlayerEffects Effects { get; }

        ref readonly RemoteServerEvents ServerEvents { get; }

        ref readonly FpsTransformComponent Transform { get; }

        IReadOnlyList<HealInfo> Heals { get; }

        bool IsAlive { get; }

        bool IsDead { get; }
    }
}