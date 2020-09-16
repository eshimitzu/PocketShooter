using Heyworks.PocketShooter.Meta.Entities;

namespace Heyworks.PocketShooter.Meta.Configuration.Data
{
    public class WeaponGradeConfig
    {
        public WeaponName Name { get; set; }

        public Grade Grade { get; set; }

        public Price InstantPrice { get; set; }

        public ItemStats Stats { get; set; }
    }
}
