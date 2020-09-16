namespace Heyworks.PocketShooter.Realtime.Entities.Skills
{
    /// <summary>
    /// Castable skill.
    /// </summary>
    public class CastableSkill : SimpleUsableCooldownSkill, ICastableSkill, ICastableSkillForSystem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CastableSkill"/> class.
        /// </summary>
        /// <param name="state">state.</param>
        public CastableSkill(ISkillRef state)
            : base(state)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether skill casting or not.
        /// </summary>
        public bool Casting
        {
            get => SkillRef.Value.Casting.Casting;
            set => SkillRef.Value.Casting.Casting = value;
        }

        /// <summary>
        /// Gets or sets a value indicating when skill casting ends.
        /// </summary>
        public int CastingExpireAt
        {
            get => SkillRef.Value.CastingExpire.CastingExpireAt;
            set => SkillRef.Value.CastingExpire.CastingExpireAt = value;
        }
    }
}