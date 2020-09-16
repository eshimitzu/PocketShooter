using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Heyworks.PocketShooter.Character;
using Heyworks.PocketShooter.Utils.Unity.EditorTools.Attributes;
using UnityEngine;
using Zenject;

namespace Heyworks.PocketShooter.Weapons
{
    /// <summary>
    /// Represents weapon view.
    /// </summary>
    public class WeaponView : MonoBehaviour
    {
        private static readonly int Finish = Animator.StringToHash("Finish");
        private static readonly int Launch = Animator.StringToHash("Launch");

        [SerializeField]
        [OptionalProperty]
        private ParticleSystem muzzleParticleSystem;

        [SerializeField]
        [OptionalProperty]
        private Animator animationController;

        [SerializeField]
        private Vector3 initPosition;
        [SerializeField]
        private Vector3 initRotation;
        [SerializeField]
        private List<MeshCollider> meshColliders = new List<MeshCollider>();
        [SerializeField]
        private Rigidbody rigidBody;
        [SerializeField]
        private Renderer[] renderers;

        private IAttackVisualizer attackVisualizer;

        public RenderView RenderView { get; private set; }

        private void Awake()
        {
            attackVisualizer = GetComponent<IAttackVisualizer>();
            SetInitTransform();

            RenderView = new RenderView(renderers);
        }

        /// <summary>
        /// Visualizes the attack.
        /// </summary>
        /// <param name="attackTarget">The attack target.</param>
        /// <param name="damageHandler">The damage handler.</param>
        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed.")]
        public void VizualizeAttack(Vector3 attackTarget, Action<Vector3> damageHandler)
        {
            StartCoroutine(VizualizeWithDelay(attackTarget, damageHandler));

            if (muzzleParticleSystem != null)
            {
                muzzleParticleSystem.Play();
            }

            if (animationController)
            {
                animationController.SetTrigger(Launch);
            }
        }

        public void ActivatePhysics(bool isActivatePhysics, int layer)
        {
            foreach (MeshCollider meshCollider in meshColliders)
            {
                meshCollider.enabled = isActivatePhysics;
                meshCollider.gameObject.layer = layer;
            }

            rigidBody.isKinematic = !isActivatePhysics;

            if (!isActivatePhysics)
            {
                SetInitTransform();
            }
        }

        public void ResetVisualization()
        {
            if (animationController)
            {
                animationController.SetTrigger(Finish);
            }
        }

        public void SetVisible(bool isVisible)
        {
            foreach (Renderer r in renderers)
            {
                r.enabled = isVisible;
            }
        }

        private IEnumerator VizualizeWithDelay(Vector3 attackTarget, Action<Vector3> damageHandler)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            attackVisualizer?.Attack(attackTarget, damageHandler);
        }

        private void SetInitTransform()
        {
            transform.localPosition = initPosition;
            transform.localRotation = Quaternion.Euler(initRotation);
        }

        public class Factory : PlaceholderFactory<UnityEngine.Object, WeaponView>
        {
        }
    }
}
