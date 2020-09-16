using UnityEngine;

namespace Heyworks.PocketShooter.Skills.Configuration
{
    [CreateAssetMenu(fileName = "InstantReloadSkillSpec", menuName = "Heyworks/Skills/Instant Reload Skill Spec")]
    public class InstantReloadSkillSpec : SkillSpec
    {
        [SerializeField]
        private Vector3 instantReloadEffectPosition;

        public Vector3 InstantReloadEffectPosition => instantReloadEffectPosition;
    }
}
