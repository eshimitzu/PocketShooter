using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Entities.Skills;

namespace Heyworks.PocketShooter.Skills
{
    public class HealSkillController : SkillController
    {
        public HealSkillController(OwnedSkill skillModel, SkillSpec spec, LocalCharacter character)
            : base(skillModel, spec, character)
        {
        }
    }
}
