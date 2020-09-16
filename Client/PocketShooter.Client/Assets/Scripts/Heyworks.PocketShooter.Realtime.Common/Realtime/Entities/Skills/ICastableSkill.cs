namespace Heyworks.PocketShooter.Realtime.Entities.Skills
{
    /// <summary>
    /// CastableSkill skill.
    /// </summary>
    public interface ICastableSkill : ICooldownSkill
    {
        /// <summary>
        /// Gets or sets a value indicating whether the skill is casting.
        /// </summary>
        bool Casting { get; }
    }
}