using System.Collections.Generic;
using Heyworks.PocketShooter.Audio;
using Heyworks.PocketShooter.Configuration;
using UnityEngine;

namespace Heyworks.PocketShooter.Character
{
    /// <summary>
    /// Represent trooper avatar class that is responsible for the trooper appearance.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class TrooperAvatar : MonoBehaviour
    {
        [SerializeField]
        private Animator animator;
        [SerializeField]
        private CharacterAnimationEventsHandler animationEventsHandler;
        [SerializeField]
        private Transform weaponViewParent;
        [SerializeField]
        private FootstepsAudioManager footstepsAudioManager;
        [SerializeField]
        private List<Rigidbody> ragdollBodies = new List<Rigidbody>();
        [SerializeField]
        private List<Collider> ragdollBodiesColliders = new List<Collider>();
        [SerializeField]
        private Renderer[] meshes;

        public Transform WeaponViewParent => weaponViewParent;

        public FootstepsAudioManager FootstepsAudioManager => footstepsAudioManager;

        public List<Rigidbody> RagdollBodies => ragdollBodies;

        public List<Collider> RagdollBodiesColliders => ragdollBodiesColliders;

        public Animator Animator => animator;

        public CharacterAnimationEventsHandler AnimationEventsHandler => animationEventsHandler;

        /// <summary>
        /// Gets. Create and set materials clones.
        /// </summary>
        public RenderView RenderView { get; private set; }

        private void Awake()
        {
            RenderView = new RenderView(meshes);
        }

        public void Setup(TrooperMaterialConfig trooperMaterialsConfig, bool isEnemy)
        {
            foreach (MaterialsSnapshot snapshot in RenderView.ClonedMaterials)
            {
                foreach (Material material in snapshot.Materials)
                {
                    material.shader = trooperMaterialsConfig.HighlightedShader;
                    material.SetRimColor(
                        isEnemy
                            ? trooperMaterialsConfig.EnemyHighlightRimColor
                            : trooperMaterialsConfig.FriendHighlightRimColor);
                    material.SetOutlineColor(
                        isEnemy
                            ? trooperMaterialsConfig.EnemyHighlightOutlineColor
                            : trooperMaterialsConfig.FriendHighlightOutlineColor);
                    material.SetRimPower(trooperMaterialsConfig.RimPower);
                    material.SetOutline(trooperMaterialsConfig.OutlineWidth);
                }
            }
        }

        public void SetMeshesVisible(bool isVisible)
        {
            foreach (var mesh in meshes)
            {
                mesh.enabled = isVisible;
            }
        }

        private void OnDestroy()
        {
            RenderView.Clean();
        }

        public void SetMeshesArray(Renderer[] meshes)
        {
            this.meshes = meshes;
        }
    }
}