using UnityEngine;

namespace Heyworks.PocketShooter.Skills.Configuration
{
    [CreateAssetMenu(fileName = "StealthDashSkillSpec", menuName = "Heyworks/Skills/Stealth Dash Skill Spec")]
    public class StealthDashSkillSpec : SkillSpec
    {
        [SerializeField]
        private InvisibilitySkillSpec invisibilitySkillSpec;

        [SerializeField]
        private ParticleSystem cameraVfxPrefab;

        public InvisibilitySkillSpec InvisibilitySkillSpec => invisibilitySkillSpec;

        public ParticleSystem CameraVfxPrefab => cameraVfxPrefab;
    }
}