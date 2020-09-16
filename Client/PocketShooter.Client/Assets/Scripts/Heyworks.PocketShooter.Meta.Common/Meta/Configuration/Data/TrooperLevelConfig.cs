using System;
using Heyworks.PocketShooter.Meta.Entities;

namespace Heyworks.PocketShooter.Meta.Configuration.Data
{
    public class TrooperLevelConfig
    {
        public TrooperClass Class { get; set; }

        public int Level { get; set; }

        public Price InstantPrice { get; set; }

        public Price RegularPrice { get; set; }

        public TimeSpan RegularDuration { get; set; }

        public ItemStats Stats { get; set; }
    }
}
