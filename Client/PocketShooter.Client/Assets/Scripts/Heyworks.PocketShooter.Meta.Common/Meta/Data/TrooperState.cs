using Heyworks.PocketShooter.Meta.Entities;

namespace Heyworks.PocketShooter.Meta.Data
{
    public class TrooperState
    {
        public int Id { get; set; }

        public TrooperClass Class { get; set; }

        public Grade Grade { get; set; }

        public int Level { get; set; }

        public WeaponName? CurrentWeapon { get; set; }

        public HelmetName? CurrentHelmet { get; set; }

        public ArmorName? CurrentArmor { get; set; }
    }
}
