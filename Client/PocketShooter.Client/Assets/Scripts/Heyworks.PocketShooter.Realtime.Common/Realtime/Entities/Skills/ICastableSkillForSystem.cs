namespace Heyworks.PocketShooter.Realtime.Entities.Skills
{
    /// <summary>
    /// CastableSkillForSystem skill.
    /// </summary>
    public interface ICastableSkillForSystem : ICooldownSkillForSystem
    {
        /// <summary>
        /// Gets or sets a value indicating whether the skill is casting.
        /// </summary>
        bool Casting { get; set; }

        /// <summary>
        /// Gets or sets a value indicating when skill casting ends.
        /// </summary>
        int CastingExpireAt { get; set; }
    }
}