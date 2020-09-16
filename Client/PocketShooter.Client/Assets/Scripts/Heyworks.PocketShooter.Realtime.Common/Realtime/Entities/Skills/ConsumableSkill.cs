namespace Heyworks.PocketShooter.Realtime.Entities.Skills
{
    /// <summary>
    /// Consumable skill.
    /// </summary>
    public class ConsumableSkill : SimpleUsableCooldownSkill, IConsumableSkill, IConsumableSkillForSystem
    {
        /// <summary>
        /// Gets or sets a value indicating number of skill uses.
        /// </summary>
        public int UseCount
        {
            get => SkillRef.Value.Consumable.UseCount;
            set => SkillRef.Value.Consumable.UseCount = value;
        }

        /// <summary>
        /// Gets or sets a value indicating number of skill uses.
        /// </summary>
        public int AvailableCount
        {
            get => SkillRef.Value.Consumable.AvailableCount;
            set => SkillRef.Value.Consumable.AvailableCount = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsumableSkill"/> class.
        /// </summary>
        /// <param name="state">state.</param>
        public ConsumableSkill(ISkillRef state)
            : base(state)
        {
        }

        /// <summary>
        /// Returns true if the skill can be used now.
        /// </summary>
        public override bool CanUseSkill()
        {
            return base.CanUseSkill() && AvailableCount > UseCount;
        }
    }
}