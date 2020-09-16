using Heyworks.PocketShooter.Character.Bot.Skills;
using Heyworks.PocketShooter.Utils;
using Heyworks.PocketShooter.Utils.Unity.EditorTools.Attributes;
using UnityEngine;

namespace Heyworks.PocketShooter.Skills
{
    [CreateAssetMenu(fileName = "GrenadeSkillSpec", menuName = "Heyworks/Skills/Grenade Skill Spec")]
    public class GrenadeSkillSpec : SkillSpec
    {
        [SerializeField]
        private Grenade grenadePrefab;

        [SerializeField]
        private GrenadeProxy grenadeProxyPrefab;

        [SerializeField]
        private TrajectoryDrawer trajectoryPrefab;

        [SerializeField]
        private float throwPower = 10f;

        [SerializeField]
        private float initialThrowAngle = -15f;

        [SerializeField]
        private Vector3 grenadePosition = Vector3.zero;

        [SerializeField]
        [Layer]
        private int allyThrowableItemLayer;

        [SerializeField]
        [Layer]
        private int enemyThrowableItemLayer;

        public Grenade GrenadePrefab => grenadePrefab;

        public GrenadeProxy GrenadeProxyPrefab => grenadeProxyPrefab;

        public TrajectoryDrawer TrajectoryPrefab => trajectoryPrefab;

        public float ThrowPower => throwPower;

        public float InitialThrowAngle => initialThrowAngle;

        public int AllyThrowableItemLayer => allyThrowableItemLayer;

        public int EnemyThrowableItemLayer => enemyThrowableItemLayer;

        public Vector3 GrenadePosition => grenadePosition;
    }
}