using Heyworks.PocketShooter.Meta.Entities;

namespace Heyworks.PocketShooter.Meta.Data
{
    public class WeaponState
    {
        public int Id { get; set; }

        public WeaponName Name { get; set; }

        public Grade Grade { get; set; }

        public int Level { get; set; }
    }
}
