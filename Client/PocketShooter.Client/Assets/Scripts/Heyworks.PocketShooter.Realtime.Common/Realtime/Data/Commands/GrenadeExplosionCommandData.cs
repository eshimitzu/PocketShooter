using Collections.Pooled;

namespace Heyworks.PocketShooter.Realtime.Data
{
    /// <summary>
    /// GrenadeExplosionCommandData.
    /// </summary>
    public sealed class GrenadeExplosionCommandData : IGameCommandData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrenadeExplosionCommandData"/> class.
        /// </summary>
        /// <param name="playerId">playerId.</param>
        /// <param name="directVictims">Direct victims.</param>
        /// <param name="splashVictims">Splash victims.</param>
        public GrenadeExplosionCommandData(EntityId playerId, PooledList<EntityId> directVictims, PooledList<EntityId> splashVictims)
        {
            ActorId = playerId;
            SplashVictims = splashVictims;
            DirectVictims = directVictims;
        }

        /// <summary>
        /// Gets player owning the command.
        /// </summary>
        public EntityId ActorId { get; }

        public SimulationDataCode Code => SimulationDataCode.GrenadeExplosion;

        /// <summary>
        /// Gets the grenade direct victims.
        /// </summary>
        public PooledList<EntityId> DirectVictims { get; }

        /// <summary>
        /// Gets the grenade splash victims.
        /// </summary>
        public PooledList<EntityId> SplashVictims { get; }
    }
}