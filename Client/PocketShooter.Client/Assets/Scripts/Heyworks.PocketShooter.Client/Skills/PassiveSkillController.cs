using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Entities.Skills;

namespace Heyworks.PocketShooter.Skills
{
    public class PassiveSkillController : SkillController
    {
        public PassiveSkillController(OwnedSkill skillModel, SkillSpec spec, LocalCharacter character)
            : base(skillModel, spec, character)
        {
        }

        public override void SkillControlOnClick()
        {
        }
    }
}
