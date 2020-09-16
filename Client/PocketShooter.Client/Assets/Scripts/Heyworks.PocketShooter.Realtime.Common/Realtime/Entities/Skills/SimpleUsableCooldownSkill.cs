namespace Heyworks.PocketShooter.Realtime.Entities.Skills
{
    /// <summary>
    /// Simple usable cooldown skill.
    /// </summary>
    public class SimpleUsableCooldownSkill : Skill, ICooldownSkill, ICooldownSkillForSystem
    {
        /// <summary>
        /// Gets or sets the timestamp when skill state should expired.
        /// </summary>
        public int StateExpireAt
        {
            get => SkillRef.Value.StateExpire.ExpireAt;
            set => SkillRef.Value.StateExpire.ExpireAt = value;
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="SimpleUsableCooldownSkill"/> class.
        /// </summary>
        /// <param name="state">State.</param>
        public SimpleUsableCooldownSkill(ISkillRef state)
               : base(state)
        {
        }
    }
}