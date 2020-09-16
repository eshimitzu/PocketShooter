using Heyworks.PocketShooter.Meta.Entities;

namespace Heyworks.PocketShooter.Meta.Data
{
    public class ArmorState
    {
        public int Id { get; set; }

        public ArmorName Name { get; set; }

        public Grade Grade { get; set; }

        public int Level { get; set; }
    }
}