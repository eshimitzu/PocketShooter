namespace Heyworks.PocketShooter.Realtime.Entities.Skills
{
    /// <summary>
    /// Skill with aiming component.
    /// </summary>
    public interface IAimingSkill : ICooldownSkill
    {
        /// <summary>
        /// Gets or sets a value indicating whether the skill is aiming.
        /// </summary>
        bool Aiming { get; }
    }
}