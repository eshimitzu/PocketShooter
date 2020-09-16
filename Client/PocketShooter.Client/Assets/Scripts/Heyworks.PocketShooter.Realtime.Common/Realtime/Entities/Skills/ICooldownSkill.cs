namespace Heyworks.PocketShooter.Realtime.Entities.Skills
{
    /// <summary>
    /// Cooldown skill.
    /// </summary>
    public interface ICooldownSkill : ISkill
    {
        /// <summary>
        /// Gets the timestamp when skill state should expired.
        /// </summary>
        int StateExpireAt { get; }
    }
}