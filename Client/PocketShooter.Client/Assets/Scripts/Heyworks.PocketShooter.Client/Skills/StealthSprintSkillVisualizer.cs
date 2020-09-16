using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Entities.Skills;
using Heyworks.PocketShooter.Skills.Configuration;

namespace Heyworks.PocketShooter.Skills
{
    public class StealthSprintSkillVisualizer : SkillVisualizer
    {
        private readonly SprintSkillVisualizer sprintSkillVisualizer;
        private readonly InvisibilitySkillVisualizer invisibilitySkillVisualizer;

        public StealthSprintSkillVisualizer(Skill skillModel, StealthSprintSkillSpec spec, NetworkCharacter character)
            : base(skillModel, spec, character)
        {
            sprintSkillVisualizer = new SprintSkillVisualizer(skillModel, spec.SprintSkillSpec, character);
            invisibilitySkillVisualizer = new InvisibilitySkillVisualizer(skillModel, spec.InvisibilitySkillSpec, character);
        }

        public override void Visualize()
        {
            sprintSkillVisualizer.Visualize();
            invisibilitySkillVisualizer.Visualize();
        }

        public override void Finish()
        {
            sprintSkillVisualizer.Finish();
            invisibilitySkillVisualizer.Finish();
        }

        public override void Cancel()
        {
            sprintSkillVisualizer.Cancel();
            invisibilitySkillVisualizer.Cancel();
        }
    }
}