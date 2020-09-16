using Heyworks.PocketShooter.Audio;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Entities.Skills;
using Heyworks.PocketShooter.Skills.Configuration;
using UniRx;

namespace Heyworks.PocketShooter.Skills
{
    public class StealthDashSkillVisualizer : SkillVisualizer
    {
        private readonly InvisibilitySkillVisualizer invisibilitySkillVisualizer;

        public StealthDashSkillVisualizer(Skill skillModel, StealthDashSkillSpec spec, NetworkCharacter character)
            : base(skillModel, spec, character)
        {
            invisibilitySkillVisualizer = new InvisibilitySkillVisualizer(
                skillModel,
                spec.InvisibilitySkillSpec,
                character);
            character.ModelEvents.DashChanged.Subscribe(DashChanged).AddTo(character);
        }

        public override void Cancel()
        {
            invisibilitySkillVisualizer.Cancel();
            FinishDash();
        }

        private void DashChanged(bool isDashing)
        {
            if (isDashing)
            {
                Character.CharacterCommon.AudioController.PostEvent(AudioKeys.Event.PlayDash);
                Character.CharacterCommon.AnimationController.StealthDashSetActive(true);
            }
            else
            {
                FinishDash();
            }
        }

        private void FinishDash()
        {
            Character.CharacterCommon.AnimationController.StealthDashSetActive(false);
        }
    }
}