namespace Heyworks.PocketShooter.Realtime.Entities.Skills
{
    /// <summary>
    /// Skill factory.
    /// </summary>
    public static class SkillFactory
    {
        /// <summary>
        /// Creates the skill.
        /// </summary>
        /// <param name="skillRef">Skill reference.</param>
        public static Skill CreateSkill(ISkillRef skillRef)
        {
            switch (skillRef.Id.SkillName)
            {
                case SkillName.MedKit:
                    return new ConsumableSkill(skillRef);
                case SkillName.Grenade:
                    return new GrenadeSkill(skillRef);
                case SkillName.Lifesteal:
                    return new LifestealSkill(skillRef);
                case SkillName.StealthDash:
                case SkillName.DoubleStealthDash:
                case SkillName.ShockWave:
                case SkillName.ShockWaveJump:
                case SkillName.Dive:
                    return new CastableSkill(skillRef);
                default:
                    return new SimpleUsableCooldownSkill(skillRef);
            }
        }
    }
}
