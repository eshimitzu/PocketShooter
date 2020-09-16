using System.Diagnostics.CodeAnalysis;
using Heyworks.PocketShooter.Character;
using Heyworks.PocketShooter.Modules.EffectsManager;
using Heyworks.PocketShooter.Realtime;
using Heyworks.PocketShooter.Realtime.Configuration;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Simulation;
using Heyworks.PocketShooter.Weapons;
using UniRx;
using UnityEngine;
using Zenject;

namespace Heyworks.PocketShooter.Networking.Actors
{
    public class RemoteCharacter : NetworkCharacter
    {
        [SerializeField]
        private GameObject aimTarget;

        [SerializeField]
        private OrbitFollowTransform orbitFollowTransform;

        [Inject]
        private IRealtimeConfiguration realtimeConfiguration;

        private SpeedInterpolator interpolator;
        private ITickProvider tickProvider;

        private WeaponRaycaster weaponRaycaster;

        private GameObject reloadEffect;

        public new IRemotePlayer Model { get; private set; }

        public OrbitFollowTransform OrbitFollowTransform => orbitFollowTransform;

        public RemoteCharacterView RemoteCharacterView => CharacterView as RemoteCharacterView;

        public void Setup(
            EntityId id,
            IRemotePlayer model,
            bool isEnemy,
            ITickProvider tickProvider,
            IClientSimulation simulation,
            ICharacterContainer characterContainer,
            float deadHitPower,
            WeaponRaycaster weaponRaycaster)
        {
            Setup(model, model.Events, simulation, characterContainer, isEnemy);
            Model = model;
            DeadHitPower = deadHitPower;
            this.weaponRaycaster = weaponRaycaster;
            this.tickProvider = tickProvider;
            interpolator = new SpeedInterpolator(tickProvider, realtimeConfiguration, model);

            orbitFollowTransform.Setup(CharacterCommon.CameraFollowPoint);

            model.Events.Moved.Subscribe(UpdateTransform).AddTo(this);
            model.Events.WeaponStateChanged.Subscribe(UpdateWeaponState).AddTo(this);
            model.Events.WarmingUpProgressChanged.Subscribe(UpdateWarmingUpProgress).AddTo(this);
        }

        protected void Update()
        {
            var (position, rotation, pitch) = interpolator.Interpolate(Time.deltaTime);
            transform.SetPositionAndRotation(position, rotation);
            orbitFollowTransform.UpdateWithRotation(Quaternion.Euler(pitch, rotation.eulerAngles.y, 0));

            // TODO: v.shimkovich one of that is used for implementation of remote player body aiming animation. Ask e.bogdan for details
            // animationController.SetPitchValue(pitch);
            // bodyAimRotationHelper.SetRotation(Quaternion.Euler(bodyRotation));
            // TODO: v.shimkovich and after that check for no gimbal lock of body rotation
        }

        [SuppressMessage(
            "StyleCop.CSharp.OrderingRules",
            "SA1202:ElementsMustBeOrderedByAccess",
            Justification = "Reviewed.")]
        protected override void HandleDamages(DamagedServerEvent dse)
        {
            RemoteCharacterView.ShowStatusBar(Model);

            base.HandleDamages(dse);
        }

        protected override void HandleDamage(in DamageInfo damage)
        {
            base.HandleDamage(in damage);

            NetworkCharacter character;

            if (damage.DamageType == DamageType.Extra)
            {
                character = CharacterContainer.GetCharacter(Simulation.Game.LocalPlayerId);
            }
            else
            {
                character = CharacterContainer.GetCharacter(damage.AttackerId);
            }

            if (character && character is LocalCharacter localCharacter)
            {
                Vector3 toLocalCharacterDirection =
                    (localCharacter.InputController.OrbitCamera.transform.position - transform.position).normalized;
                Vector3 pos = (damage.DamageType == DamageType.Critical || damage.DamageType == DamageType.Extra)
                    ? CharacterCommon.HeadHintTransform.position
                    : CharacterCommon.BodyHintTransform.position;

                DamageHintController.ShowHint(
                    pos,
                    StringNumbersCache.GetString((int)damage.Damage),
                    damage.DamageType,
                    toLocalCharacterDirection);
            }
        }

        protected override void UpdateExpendables(EntityId any)
        {
            if (Model.IsAlive)
            {
                RemoteCharacterView.ShowStatusBar(Model);
            }

            base.UpdateExpendables(any);
        }

        protected override void CharacterDeath(KilledServerEvent kse)
        {
            base.CharacterDeath(kse);

            ActiveRagdoll(kse.ActorId);

            RemoteCharacterView.HideStatusBarImmediately();

            StopReload();
        }

        protected override void ActiveRagdoll(EntityId killerID)
        {
            if (aimTarget != null)
            {
                aimTarget.SetActive(false);
            }

            base.ActiveRagdoll(killerID);
        }

        private void UpdateWarmingUpProgress(float progress) =>
            CharacterCommon.AnimationController.Aim(progress > 0);

        private void UpdateTransform(FpsTransformComponent fpsTransform) => interpolator.Update(fpsTransform);

        private void UpdateWeaponState(WeaponStateChangedEvent e)
        {
            switch (e.Next)
            {
                case WeaponState.Attacking:
                    CharacterCommon.AnimationController.Shoot();
                    break;
                case WeaponState.Reloading:
                    reloadEffect = EffectsManager.Instance.PlayEffect(
                        EffectType.Reload,
                        transform,
                        new Vector3(0.136f, 1.488f, 0),
                        true);

                    CharacterCommon.AnimationController.Reload();
                    break;
            }

            if (e.Next != WeaponState.Reloading)
            {
                StopReload();
            }
        }

        private void StopReload()
        {
            if (reloadEffect)
            {
                EffectsManager.Instance.StopEffect(reloadEffect);
                reloadEffect = null;
            }
        }
    }
}