using System;
using System.Diagnostics.CodeAnalysis;
using Heyworks.PocketShooter.Camera;
using Heyworks.PocketShooter.Character;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Simulation;
using Heyworks.PocketShooter.Skills;
using UniRx;
using UnityEngine;
using Zenject;

namespace Heyworks.PocketShooter.Weapons
{
    public class WeaponScopeBehaviour : MonoBehaviour, IInputSensetivityProvider
    {
        [SerializeField]
        private WeaponViewSettings weaponViewSettings;

        [SerializeField]
        private LocalCharacter localCharacter;

        [Inject]
        private OrbitCamera orbitCamera;

        public event Action ScopeOut;

        public bool IsScopeActive { get; set; }

        private WeaponViewSettings.WeaponViewParameters CurrentViewParameters
        {
            get
            {
                if (IsScopeActive)
                {
                    return weaponViewSettings.GetWeaponViewParameters(localCharacter.Model.CurrentWeapon.Name);
                }

                return weaponViewSettings.GetDefaultWeaponViewParameters();
            }
        }

        public float InputSensetivityMultiplier => CurrentViewParameters.ZoomControlSensetivityMultiplier;

        private void Start()
        {
            localCharacter.ModelEvents.Killed.Subscribe(LocalCharacter_OnKilled).AddTo(this);
            localCharacter.ModelEvents.StunChanged.Subscribe(UpdateStun).AddTo(this);

            localCharacter.ModelEvents.SkillStateChanged.Subscribe(SkillStateChanged).AddTo(this);
        }

        private void OnDestroy()
        {
            Use(false);
        }

        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed.")]
        public void Use(bool value)
        {
            IsScopeActive = value;

            localCharacter.CharacterCommon.TrooperAvatar.SetMeshesVisible(!value);
            localCharacter.CharacterView.WeaponView.SetVisible(!value);

            orbitCamera.UpdateCameraSetings(CurrentViewParameters);
        }

        private void LocalCharacter_OnKilled(KilledServerEvent kse)
        {
            ScopeOut?.Invoke();
            Use(false);
        }

        private void UpdateStun(bool isStunned)
        {
            if (isStunned)
            {
                ScopeOut?.Invoke();
                Use(false);
            }
        }

        protected virtual void SkillStateChanged(SkillStateChangedEvent sse)
        {
            if (sse.Activated)
            {
                if (!((localCharacter.FirstSkillController is PassiveSkillController psc1 && psc1.SkillName == sse.SkillName) ||
                     (localCharacter.SecondSkillController is PassiveSkillController psc2 && psc2.SkillName == sse.SkillName) ||
                     (localCharacter.ThirdSkillController is PassiveSkillController psc3 && psc3.SkillName == sse.SkillName) ||
                     (localCharacter.FourthSkillController is PassiveSkillController psc4 && psc4.SkillName == sse.SkillName) ||
                     (localCharacter.FifthSkillController is PassiveSkillController psc5 && psc5.SkillName == sse.SkillName)))
                {
                    ScopeOut?.Invoke();
                    Use(false);
                }
            }
        }
    }
}