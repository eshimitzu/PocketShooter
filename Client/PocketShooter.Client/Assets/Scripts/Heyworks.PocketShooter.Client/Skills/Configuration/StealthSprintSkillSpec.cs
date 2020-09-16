using UnityEngine;

namespace Heyworks.PocketShooter.Skills.Configuration
{
    [CreateAssetMenu(fileName = "StealthSprintSkillSpec", menuName = "Heyworks/Skills/Stealth Sprint Skill Spec")]
    public class StealthSprintSkillSpec : SkillSpec
    {
        [SerializeField]
        private SprintSkillSpec sprintSkillSpec;

        [SerializeField]
        private InvisibilitySkillSpec invisibilitySkillSpec;

        public SprintSkillSpec SprintSkillSpec => sprintSkillSpec;

        public InvisibilitySkillSpec InvisibilitySkillSpec => invisibilitySkillSpec;
    }
}