using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Entities.Skills
{
    public class OwnedSkill
    {
        public SkillInfo Info { get; }

        public Skill Skill { get; }

        public SkillState State => Skill.State;

        public SkillName Name => Skill.Name;

        /// <summary>
        /// Initializes a new instance of the <see cref="OwnedSkill"/> class.
        /// </summary>
        /// <param name="skill">Skill reference.</param>
        /// <param name="skillInfo">The information about skill.</param>
        public OwnedSkill(Skill skill, SkillInfo skillInfo)
        {
            Skill = skill;
            Info = skillInfo;
        }

        /// <summary>
        /// Returns true if the skill can be used now.
        /// </summary>
        public bool CanUseSkill()
        {
            return Skill.CanUseSkill();
        }
    }
}
