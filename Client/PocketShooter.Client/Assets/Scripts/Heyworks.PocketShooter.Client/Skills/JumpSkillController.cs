using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Entities.Skills;

namespace Heyworks.PocketShooter.Skills
{
    public class JumpSkillController : SkillController
    {
        private readonly Jump jump;

        public JumpSkillController(OwnedSkill skillModel, JumpSkillSpec spec, LocalCharacter character)
            : base(skillModel, spec, character)
        {
            jump = new Jump(character, spec.Angle, spec.Speed);
        }

        public override void SkillControlOnClick()
        {
            base.SkillControlOnClick();

            jump.Execute();
        }
    }
}