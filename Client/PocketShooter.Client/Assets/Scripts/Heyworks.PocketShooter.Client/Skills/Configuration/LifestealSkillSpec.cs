using UnityEngine;

namespace Heyworks.PocketShooter.Skills
{
    [CreateAssetMenu(fileName = "LifestealSkillSpec", menuName = "Heyworks/Skills/Lifesteal Skill Spec")]
    public class LifestealSkillSpec : SkillSpec
    {
        [SerializeField]
        private Vector3 lifestealEffectSkullsPosition;

        public Vector3 LifestealEffectSkullsPosition => lifestealEffectSkullsPosition;
    }
}