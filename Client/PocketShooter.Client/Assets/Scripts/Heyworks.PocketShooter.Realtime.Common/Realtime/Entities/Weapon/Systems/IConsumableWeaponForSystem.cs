namespace Heyworks.PocketShooter.Realtime.Entities.Weapon.Systems
{
    /// <summary>
    /// Represents interface for weapon that is used by consumable systems.
    /// </summary>
    internal interface IConsumableWeaponForSystem : IWeaponForSystem
    {
        /// <summary>
        /// Gets or sets the ammo in clip.
        /// </summary>
        int AmmoInClip { get; set; }
    }
}
