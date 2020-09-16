using System;
using System.Diagnostics.CodeAnalysis;
using Heyworks.PocketShooter.Client;
using Heyworks.PocketShooter.Configuration;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Utils;
using Heyworks.PocketShooter.Weapons;
using UnityEngine;
using Zenject;

namespace Heyworks.PocketShooter.Character
{
    public class CharacterView : MonoBehaviour
    {
        [SerializeField]
        private NetworkCharacter networkCharacter;

        [SerializeField]
        private CharacterCommon characterCommon;

        [SerializeField]
        private WeaponsConfig weaponsConfig;

        [SerializeField]
        private LifestealEffectController lifestealEffectController;

        [SerializeField]
        private SprintEffectController sprintEffectController;

        private WeaponView.Factory weaponViewFactory;

        public WeaponView WeaponView { get; set; }

        public LifestealEffectController LifestealEffectController => lifestealEffectController;

        public SprintEffectController SprintEffectController => sprintEffectController;

        private void OnDestroy()
        {
            if (WeaponView != null)
            {
                Destroy(WeaponView.gameObject);
            }
        }

        private void Start()
        {
            if (networkCharacter != null)
            {
                var effectControllers = GetComponentsInChildren<EffectController>();
                foreach (EffectController controller in effectControllers)
                {
                    controller.Setup(networkCharacter);
                }
            }
        }

        /// <summary>
        /// Initializes the specified weapon factory.
        /// </summary>
        /// <param name="weaponFactory">The weapon factory.</param>
        [Inject]
        [SuppressMessage(
            "StyleCop.CSharp.OrderingRules",
            "SA1202:ElementsMustBeOrderedByAccess",
            Justification = "Reviewed.")]
        public void Init(WeaponView.Factory weaponFactory)
        {
            AssertUtils.NotNull(weaponFactory, nameof(weaponFactory));

            weaponViewFactory = weaponFactory;
        }

        public void StopEffects()
        {
            var effectControllers = GetComponentsInChildren<EffectController>();
            foreach (EffectController controller in effectControllers)
            {
                controller.Stop();
            }
        }

        public void VizualizeAttack(Vector3[] targetsPositions, Action<Vector3> onHit)
        {
            foreach (var targetPosition in targetsPositions)
            {
                WeaponView.VizualizeAttack(targetPosition, onHit);
            }
        }

        public void VizualizeAttack(Vector3[] targetsPositions)
        {
            VizualizeAttack(targetsPositions, null);
        }

        public void ResetVisualization()
        {
            WeaponView.ResetVisualization();
        }

        /// <summary>
        /// Setup weapon view (temporary here TODO: replace in task trooper classes).
        /// </summary>
        /// <param name="weaponName">Weapon name.</param>
        /// <param name="trooperAvatar">Trooper avatar.</param>
        public void SetupWeaponView(WeaponName weaponName, TrooperAvatar trooperAvatar)
        {
            WeaponViewConfig weaponConfig = weaponsConfig.GetWeaponByName(weaponName);

            // TODO: a.dezhurko Remove duplication with Trooper creator
            WeaponView = weaponViewFactory.Create(weaponConfig.View);
            WeaponView.transform.SetParent(trooperAvatar.WeaponViewParent, false);
            trooperAvatar.Animator.runtimeAnimatorController = weaponConfig.CharacterAnimatorController;
        }

        public void ThrowWeaponView()
        {
            WeaponView.transform.parent = null;
            WeaponView.ActivatePhysics(true, LayerMask.NameToLayer("Default"));
        }

        public void SetWeaponViewVisible(bool isVisible)
        {
            WeaponView.SetVisible(isVisible);
        }
    }
}