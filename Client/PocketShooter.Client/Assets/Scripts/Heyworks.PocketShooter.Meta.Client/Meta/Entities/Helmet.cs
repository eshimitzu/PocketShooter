using System.Collections.Generic;
using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Data;
using Heyworks.PocketShooter.Meta.Runnables;

namespace Heyworks.PocketShooter.Meta.Entities
{
    public class Helmet : HelmetBase
    {
        private readonly IHelmetConfigurationBase helmetConfiguration;
        private readonly IEnumerable<IRunnable> runnablesCache;

        public Helmet(HelmetState helmetState, IHelmetConfigurationBase helmetConfiguration)
            : base(helmetState, helmetConfiguration)
        {
            this.helmetConfiguration = helmetConfiguration;
        }

        public ItemStats NextLevelStats =>
            !IsMaxLevel
                ? ItemStats.Sum(
                    helmetConfiguration.GetGradeStats(Name, Grade),
                    helmetConfiguration.GetLevelStats(Name, Level + 1))
                : Stats;


        public ItemStats NextGradeStats =>
            !Grade.IsMax()
                ? ItemStats.Sum(
                    helmetConfiguration.GetGradeStats(Name, Grade + 1),
                    helmetConfiguration.GetLevelStats(Name, Level))
                : Stats;

        public int NextGradeMaxLevel => Grade.IsMax() ? MaxLevel : helmetConfiguration.GetMaxLevel(Grade + 1);
    }
}