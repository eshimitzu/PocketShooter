namespace Heyworks.PocketShooter.Realtime.Data
{
    public struct RageComponents
    {
        public RageBaseComponent Base;
        public RageExpireComponent Expire;
    }

    public struct RageBaseComponent : IForAll
    {
        public int AdditionalDamagePercent;
    }

    public struct RageExpireComponent : IForOwner
    {
        public int IncreaseDamageAt;

        public int DecreaseDamageAt;

        public WeaponState LastWeaponState;
    }
}
