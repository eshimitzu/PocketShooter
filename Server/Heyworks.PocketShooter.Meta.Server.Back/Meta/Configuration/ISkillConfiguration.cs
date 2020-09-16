using Heyworks.PocketShooter.Realtime.Configuration.Data;

namespace Heyworks.PocketShooter.Meta.Configuration
{
    public interface ISkillConfiguration
    {
        /// <summary>
        /// Gets a skill realtime stats.
        /// </summary>
        /// <param name="skillName">The skill name.</param>
        /// <param name="grade">The skill's grade.</param>
        /// <exception cref="System.InvalidOperationException">If a stats is not found.</exception>
        SkillStatsConfig GetRealtimeStats(SkillName skillName, Grade grade);
    }
}
