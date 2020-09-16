using System.Collections;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Entities.Skills;
using Heyworks.PocketShooter.Realtime.Simulation;
using Heyworks.PocketShooter.Utils;
using Heyworks.PocketShooter.Utils.Extensions;
using Lean.Pool;
using UniRx;
using UnityEngine;

namespace Heyworks.PocketShooter.Skills
{
    public class GrenadeSkillVisualizer : SkillVisualizer
    {
        private new GrenadeSkillSpec Spec { get; }

        private new GrenadeSkill Model { get; }

        private Transform rightHand;
        private Grenade grenadeInstance;

        public GrenadeSkillVisualizer(GrenadeSkill skillModel, GrenadeSkillSpec spec, NetworkCharacter character)
            : base(skillModel, spec, character)
        {
            Spec = spec;
            Model = skillModel;

            rightHand = Character.CharacterCommon.TrooperAvatar.Animator.GetBoneTransform(
                HumanBodyBones.RightMiddleProximal);

            character.ModelEvents.SkillAimChanged.Where(e => e.SkillName == SkillName.Grenade)
                .Subscribe(AimChanged)
                .AddTo(character);

            Character.CharacterCommon.TrooperAvatar.AnimationEventsHandler.OnThrowGrenade +=
                AnimationEventsHandler_OnThrowGrenade;

            Character.CharacterCommon.TrooperAvatar.AnimationEventsHandler.OnEndThrowGrenade +=
                AnimationEventsHandler_OnEndThrowGrenade;
        }

        public override void Visualize()
        {
            Character.CharacterView.SetWeaponViewVisible(false);
            Character.CharacterCommon.AnimationController.GrenadeThrow();
        }

        public override void Cancel()
        {
            Character.CharacterView.SetWeaponViewVisible(true);

            if (grenadeInstance != null)
            {
                UnityEngine.Object.Destroy(grenadeInstance.gameObject);
                grenadeInstance = null;
            }
        }

        private void AimChanged(SkillAimChangeEvent e)
        {
            Character.CharacterCommon.AnimationController.GrenadeAim(e.IsAiming);

            if (e.IsAiming)
            {
                Character.CharacterView.SetWeaponViewVisible(false);
                SpawnGrenade();
            }
        }

        private void SpawnGrenade()
        {
            grenadeInstance = LeanPool.Spawn(
                Spec.GrenadePrefab,
                Spec.GrenadePosition,
                Quaternion.identity,
                rightHand);

            grenadeInstance.Setup(Character);
            grenadeInstance.gameObject.layer = GetGrenadeLayer();
            grenadeInstance.gameObject.SetActive(true);
            grenadeInstance.TrailRenderer.enabled = false;
            Rigidbody rigidBody = grenadeInstance.Rigidbody;
            rigidBody.isKinematic = true;
            grenadeInstance.Collider.enabled = false;
        }

        private void ThrowGrenade()
        {
            Rigidbody rigidBody = grenadeInstance.Rigidbody;

            float aimAngle = Character.Model.Transform.Pitch;
            Quaternion throwAngle = Quaternion.Euler(Spec.InitialThrowAngle + aimAngle, 0f, 0f);
            Vector3 force = (Character.transform.rotation * throwAngle) * new Vector3(0, 0, Spec.ThrowPower);
            grenadeInstance.TrailRenderer.enabled = true;
            rigidBody.isKinematic = false;
            grenadeInstance.Collider.enabled = true;
            grenadeInstance.transform.parent = null;
            rigidBody.velocity = Vector3.zero;
            rigidBody.AddForce(force, ForceMode.VelocityChange);
        }

        private int GetGrenadeLayer()
        {
            return Character.IsEnemy ? Spec.EnemyThrowableItemLayer : Spec.AllyThrowableItemLayer;
        }

        private void AnimationEventsHandler_OnThrowGrenade()
        {
            if (grenadeInstance == null)
            {
                SpawnGrenade();
            }

            ThrowGrenade();
            grenadeInstance = null;
        }

        private void AnimationEventsHandler_OnEndThrowGrenade()
        {
            Character.CharacterView.SetWeaponViewVisible(true);
        }
    }
}