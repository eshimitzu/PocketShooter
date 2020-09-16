namespace Heyworks.PocketShooter.Realtime.Data
{
    public class ItemsMetaInfo
    {
        private ItemsMetaInfo()
        {
        }

        public ItemsMetaInfo(WeaponName weaponName, int weaponPower, HelmetName helmetName, int helmetPower, ArmorName armorName, int armorPower)
        {
            WeaponName = weaponName;
            WeaponPower = weaponPower;
            HelmetName = helmetName;
            HelmetPower = helmetPower;
            ArmorName = armorName;
            ArmorPower = armorPower;
        }

        public WeaponName WeaponName { get; private set; }

        public int WeaponPower { get; private set; }

        public HelmetName HelmetName { get; private set; }

        public int HelmetPower { get; private set; }

        public ArmorName ArmorName { get; private set; }

        public int ArmorPower { get; private set; }
    }
}
