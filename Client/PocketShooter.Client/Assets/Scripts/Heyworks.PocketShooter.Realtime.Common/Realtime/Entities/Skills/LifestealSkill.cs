namespace Heyworks.PocketShooter.Realtime.Entities.Skills
{
    /// <summary>
    /// LifeSteal skill.
    /// </summary>
    public class LifestealSkill : SimpleUsableCooldownSkill, ILifestealSkill, ILifestealSkillForSystem
    {
        /// <inheritdoc cref="ILifestealSkillForSystem" />
        public int NextStealTime
        {
            get => SkillRef.Value.Lifesteal.NextStealTime;
            set => SkillRef.Value.Lifesteal.NextStealTime = value;
        }

        /// <inheritdoc cref="ILifestealSkillForSystem" />
        public float Radius
        {
            get => SkillRef.Value.Lifesteal.Radius;
            set => SkillRef.Value.Lifesteal.Radius = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LifestealSkill"/> class.
        /// </summary>
        /// <param name="state">Skill state reference.</param>
        public LifestealSkill(ISkillRef state)
            : base(state)
        {
        }
    }
}