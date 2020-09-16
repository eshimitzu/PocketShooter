namespace Heyworks.PocketShooter.Realtime.Entities.Skills
{
    /// <summary>
    /// Lifesteal Skill.
    /// </summary>
    public interface ILifestealSkill : ICooldownSkill
    {
        /// <summary>
        /// Gets next time of the lifesteal action.
        /// </summary>
        int NextStealTime { get; }

        /// <summary>
        /// Gets radius of lifesteal zone.
        /// </summary>
        float Radius { get; }
    }
}