namespace Heyworks.PocketShooter.Realtime.Entities.Skills
{
    /// <summary>
    /// Grenade skill.
    /// </summary>
    public class GrenadeSkill : ConsumableSkill, IAimingSkill, IAimingSkillForSystem
    {
        /// <inheritdoc cref="IAimingSkill"/>
        public bool Aiming
        {
            get => SkillRef.Value.Aiming.Aiming;
            set => SkillRef.Value.Aiming.Aiming = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GrenadeSkill"/> class.
        /// </summary>
        /// <param name="state">state.</param>
        public GrenadeSkill(ISkillRef state)
            : base(state)
        {
        }
    }
}