using Heyworks.PocketShooter.Meta.Configuration.Data;
using System.Collections.Generic;
using System.Linq;

namespace Heyworks.PocketShooter.Meta.Configuration
{
    public static class CommonConfiguration
    {
        public static Grade MaxGrade => Grade.Legendary;

        public static bool IsMax(this Grade grade) => grade == MaxGrade;

        public static int GetMaxLevel(this IEnumerable<GradeConfig> gradesConfig, Grade grade)
        {
            var gradeConfig = gradesConfig.SingleOrDefault(_ => _.Grade == grade);
            if (gradeConfig == null)
            {
                throw new ConfigurationException($"Can't find grade config for grade {grade}");
            }

            return gradeConfig.MaxLevel;
        }
    }
}
