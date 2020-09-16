namespace Heyworks.PocketShooter.Realtime.Entities.Weapon
{
    /// <summary>
    /// Data stored only locally for entity which controls weapon.
    /// </summary>
    public interface IConsumableWeapon : IWeapon
    {
        /// <summary>
        /// Gets the ammo in clip.
        /// </summary>
        int AmmoInClip { get; }
    }
}