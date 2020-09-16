using UnityEngine;

namespace Heyworks.PocketShooter.Skills.Configuration
{
    [CreateAssetMenu(fileName = "ShockWaveJumpSkillSpec", menuName = "Heyworks/Skills/Shock Wave Jump Skill Spec")]
    public class ShockWaveJumpSkillSpec : SkillSpec
    {
        [SerializeField]
        private float angle;

        [SerializeField]
        private float speed;

        [SerializeField]
        private GameObject vfxPrefab;

        public float Angle
        {
            get => angle;
            set => angle = value;
        }

        public float Speed
        {
            get => speed;
            set => speed = value;
        }

        public GameObject VfxPrefab => vfxPrefab;
    }
}
