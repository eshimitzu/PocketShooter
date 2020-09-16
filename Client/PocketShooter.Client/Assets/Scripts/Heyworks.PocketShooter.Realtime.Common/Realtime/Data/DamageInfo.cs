using Heyworks.Realtime.Serialization;

namespace Heyworks.PocketShooter.Realtime.Data
{
    /// <summary>
    /// Describes damages done.
    /// </summary>
    public readonly struct DamageInfo
    {
        /// <summary>
        /// Attacker player identifier.
        /// </summary>
        public readonly EntityId AttackerId;

        /// <summary>
        /// DamageSource.
        /// </summary>
        public readonly EntityRef DamageSource;

        /// <summary>
        /// damage type.
        /// </summary>
        [Limit(typeof(DamageType))]
        public readonly DamageType DamageType;

        /// <summary>
        /// Gets damage amount.
        /// </summary>
        public readonly float Damage;

        public DamageInfo(EntityId attackerId, EntityRef damageSource, DamageType damageType, float damage)
        {
            this.AttackerId = attackerId;
            this.DamageSource = damageSource;
            this.DamageType = damageType;
            this.Damage = damage;
        }

        public override string ToString() => $"{nameof(DamageInfo)}{(AttackerId, DamageSource, DamageType, Damage)}";
    }
}