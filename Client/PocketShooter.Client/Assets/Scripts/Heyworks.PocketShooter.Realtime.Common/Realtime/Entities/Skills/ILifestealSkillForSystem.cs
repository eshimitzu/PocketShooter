namespace Heyworks.PocketShooter.Realtime.Entities.Skills
{
    /// <summary>
    /// Lifesteal skill for system.
    /// </summary>
    public interface ILifestealSkillForSystem : ICooldownSkillForSystem
    {
        /// <summary>
        /// Gets or sets next time of the lifesteal action.
        /// </summary>
        int NextStealTime { get; set; }

        /// <summary>
        /// Gets or sets radius of lifesteal zone.
        /// </summary>
        float Radius { get; set; }
    }
}