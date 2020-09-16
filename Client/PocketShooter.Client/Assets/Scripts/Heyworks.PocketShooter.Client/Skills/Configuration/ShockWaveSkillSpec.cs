using UnityEngine;

namespace Heyworks.PocketShooter.Skills
{
    [CreateAssetMenu(fileName = "SkillSpecShockwave", menuName = "Heyworks/Skills/Skill Spec Shockwave")]
    public class ShockWaveSkillSpec : SkillSpec
    {
        [SerializeField]
        private GameObject airHornPrefab;

        [SerializeField]
        private Vector3 airHornPosition;

        [SerializeField]
        private Vector3 airHornRotation;

        public GameObject AirHornPrefab => airHornPrefab;

        public Vector3 AirHornPosition => airHornPosition;

        public Vector3 AirHornRotation => airHornRotation;
    }
}
