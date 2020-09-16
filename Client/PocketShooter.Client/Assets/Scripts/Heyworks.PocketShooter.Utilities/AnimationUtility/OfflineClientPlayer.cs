namespace Heyworks.PocketShooter.Utilities.AnimationUtility
{
    public class OfflineClientPlayer
    {
        public TrooperClass TrooperClass { get; private set; }

        public WeaponName WeaponName { get; private set; }

        public float MaxKinematicSpeed { get; private set; }

        public OfflineClientPlayer(TrooperClass trooperClass, WeaponName weaponName, float maxKinematicSpeed)
        {
            TrooperClass = trooperClass;
            this.WeaponName = weaponName;
            MaxKinematicSpeed = maxKinematicSpeed;
        }
    }
}