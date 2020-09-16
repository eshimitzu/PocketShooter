using UnityEngine;

namespace Heyworks.PocketShooter.Skills
{
    [CreateAssetMenu(fileName = "SkillSpecJump", menuName = "Heyworks/Skills/Skill Spec Jump")]
    public class JumpSkillSpec : SkillSpec
    {
        [SerializeField]
        private float angle;

        [SerializeField]
        private float speed;

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
    }
}
