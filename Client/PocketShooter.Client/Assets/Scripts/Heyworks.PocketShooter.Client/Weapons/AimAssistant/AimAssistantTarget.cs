using Heyworks.PocketShooter.Utils.Unity.EditorTools.Attributes;
using UnityEngine;

namespace Heyworks.PocketShooter.Weapons.AimAssistant
{
    public class AimAssistantTarget : MonoBehaviour
    {
        [SerializeField]
        private Collider headCollider;
        [SerializeField]
        private Collider bodyCollider;
        [SerializeField]
        private Collider selfCollider;

        [SerializeField]
        [Layer]
        private int aimAssistLayer = 0;

        public Collider HeadCollider => headCollider;

        public Collider BodyCollider => bodyCollider;

        public Collider SelfCollider => selfCollider;

        private void Start()
        {
            gameObject.layer = aimAssistLayer;
        }
    }
}
