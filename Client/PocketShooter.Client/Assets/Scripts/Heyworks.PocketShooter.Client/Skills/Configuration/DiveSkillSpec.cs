using UnityEngine;

namespace Heyworks.PocketShooter.Skills
{
    [CreateAssetMenu(fileName = "DiveSkillSpec", menuName = "Heyworks/Skills/Dive Skill Spec")]
    public class DiveSkillSpec : SkillSpec
    {
        [SerializeField]
        private GameObject diveButtonPrefab;

        [SerializeField]
        private Vector3 diveButtonPosition;

        [SerializeField]
        private Vector3 diveButtonRotation;

        [SerializeField]
        private GameObject rocketPrefab;

        [SerializeField]
        private Vector3 rocketPosition;

        [SerializeField]
        private float rocketFlyingTime;

        public GameObject DiveButtonPrefab => diveButtonPrefab;

        public Vector3 DiveButtonPosition => diveButtonPosition;

        public Vector3 DiveButtonRotation => diveButtonRotation;

        public GameObject RocketPrefab => rocketPrefab;

        public Vector3 RocketPosition => rocketPosition;

        public float RocketFlyingTime => rocketFlyingTime;
    }
}
