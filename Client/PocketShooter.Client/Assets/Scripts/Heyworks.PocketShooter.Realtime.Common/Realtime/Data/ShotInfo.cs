using Heyworks.Realtime.Serialization;

namespace Heyworks.PocketShooter.Realtime.Data
{
    /// <summary>
    /// Provides attack hits information.
    /// </summary>
    public readonly struct ShotInfo
    {
        /// <summary>
        /// Id of attacked player.
        /// </summary>
        public readonly EntityId AttackedId;

        /// <summary>
        /// The weapon name.
        /// </summary>
        [Limit(typeof(WeaponName))]
        public readonly WeaponName WeaponName;

        /// <summary>
        /// Was shot headshot.
        /// </summary>
        public readonly bool IsHeadshot;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShotInfo"/> struct.
        /// </summary>
        /// <param name="attackedId">Attacked identifier.</param>
        /// <param name="weaponName">Weapon name.</param>
        /// <param name="isHeadshot">If set to <c>true</c> was headshot.</param>
        public ShotInfo(EntityId attackedId, WeaponName weaponName,  bool isHeadshot)
        {
            this.WeaponName = weaponName;
            this.AttackedId = attackedId;
            this.IsHeadshot = isHeadshot;
        }

        public override string ToString() => $"{nameof(ShotInfo)}{(AttackedId, WeaponName, IsHeadshot)}";
    }
}