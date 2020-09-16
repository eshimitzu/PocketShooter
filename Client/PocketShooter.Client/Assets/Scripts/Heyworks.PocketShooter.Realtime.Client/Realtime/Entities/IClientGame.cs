using System.Collections.Generic;
using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Entities
{
    public interface IClientGame : IGame
    {
        DominationModeInfo ModeInfo { get; }

        IReadOnlyDictionary<EntityId, RemotePlayer> Players { get; }

        ClientPlayer LocalPlayer { get; }

        /// <summary>
        /// Gets the zones.
        /// </summary>
        IReadOnlyDictionary<byte, IZone> Zones { get; }
    }
}