namespace Heyworks.PocketShooter.Realtime.Entities.Skills
{
    /// <summary>
    /// Skill with aiming component for system.
    /// </summary>
    public interface IAimingSkillForSystem : ICooldownSkillForSystem
    {
        /// <summary>
        /// Gets or sets a value indicating whether the skill is aiming.
        /// </summary>
        bool Aiming { get; set; }
    }
}