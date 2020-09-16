namespace Heyworks.PocketShooter.Realtime.Entities.Skills
{
    /// <summary>
    /// Cooldown skill.
    /// </summary>
    public interface ICooldownSkillForSystem : ISkillForSystem
    {
        /// <summary>
        /// Gets or sets timestamp when skill state should expired.
        /// </summary>
        int StateExpireAt { get; set; }
    }
}
