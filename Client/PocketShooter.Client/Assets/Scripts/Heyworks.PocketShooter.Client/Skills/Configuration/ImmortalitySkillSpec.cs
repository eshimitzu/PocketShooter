using UnityEngine;

namespace Heyworks.PocketShooter.Skills
{
    [CreateAssetMenu(fileName = "ImmortalitySkillSpec", menuName = "Heyworks/Skills/Immortality Skill Spec")]
    public class ImmortalitySkillSpec : SkillSpec
    {
        [SerializeField]
        private Vector3 immortalyEffectPoint;

        [SerializeField]
        private float returingTimeToIdle;

        public Vector3 ImmortalyEffectPoint => immortalyEffectPoint;

        public float ReturingTimeToIdle => returingTimeToIdle;
    }
}