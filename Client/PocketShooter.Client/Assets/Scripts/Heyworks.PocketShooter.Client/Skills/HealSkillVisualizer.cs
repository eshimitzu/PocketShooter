using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Entities.Skills;

namespace Heyworks.PocketShooter.Skills
{
    public class HealSkillVisualizer : SkillVisualizer
    {
        public HealSkillVisualizer(Skill skillModel, HealSkillSpec spec, NetworkCharacter character)
            : base(skillModel, spec, character)
        {
        }
    }
}
