using Heyworks.PocketShooter.Meta.Entities;

namespace Heyworks.PocketShooter.Meta.Configuration.Data
{
    public class TrooperGradeConfig
    {
        public TrooperClass Class { get; set; }

        public Grade Grade { get; set; }

        public Price InstantPrice { get; set; }

        public ItemStats Stats { get; set; }
    }
}
