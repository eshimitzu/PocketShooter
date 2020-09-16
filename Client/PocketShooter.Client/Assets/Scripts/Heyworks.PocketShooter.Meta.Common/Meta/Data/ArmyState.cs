using System.Collections.Generic;

namespace Heyworks.PocketShooter.Meta.Data
{
    public class ArmyState
    {
        public IList<TrooperState> Troopers { get; set; } = new List<TrooperState>();

        public IList<WeaponState> Weapons { get; set; } = new List<WeaponState>();

        public IList<HelmetState> Helmets { get; set; } = new List<HelmetState>();

        public IList<ArmorState> Armors { get; set; } = new List<ArmorState>();

        public IList<OffensiveState> Offensives { get; set; } = new List<OffensiveState>();

        public IList<SupportState> Supports { get; set; } = new List<SupportState>();

        public ArmyItemProgressState ItemProgress { get; set; }

        public int NextItemId { get; set; }
    }
}
