using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Entities.Skills;

namespace Heyworks.PocketShooter.Skills
{
    public class SprintSkillVisualizer : SkillVisualizer
    {
        private new SprintSkillSpec Spec { get; }

        public SprintSkillVisualizer(Skill skillModel, SprintSkillSpec spec, NetworkCharacter character)
            : base(skillModel, spec, character)
        {
            Spec = spec;
        }

        public override void Visualize()
        {
            Character.CharacterView.SprintEffectController.Play();
        }

        public override void Finish()
        {
            Character.CharacterView.SprintEffectController.Stop();
        }
    }
}