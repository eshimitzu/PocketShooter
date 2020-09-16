using System;
using System.Diagnostics.CodeAnalysis;
using Heyworks.PocketShooter.AnimationUtility;
using UniRx;
using UnityEngine;

namespace Heyworks.PocketShooter.Character
{
    /// <summary>
    /// Represents animation controller for setting character speed and direction values.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class AnimationController : MonoBehaviour
    {
        private const string SpeedXParameter = "SpeedX";
        private const string SpeedZParameter = "SpeedZ";
        private const string PitchParameter = "Pitch";
        private const string ReloadTrigger = "Reload";
        private const string ShootTrigger = "Shoot";
        private const string AimingToggle = "Aiming";
        private const string StunnedToggle = "Stunned";

        // grenade
        private readonly AnimatorParameter aimGrenadeToggle = new AnimatorParameter("AimGrenade");
        private readonly AnimatorParameter throwGrenadeToggle = new AnimatorParameter("ThrowGrenade");

        // jump
        private readonly AnimatorParameter jumpToggle = new AnimatorParameter("Jump");
        private readonly AnimatorParameter landToggle = new AnimatorParameter("Land");

        // dash
        private readonly AnimatorParameter stealthDashBool = new AnimatorParameter("StealthDash");

        private readonly AnimatorParameter shockwaveToggle = new AnimatorParameter("Shockwave");
        private readonly AnimatorParameter shockwaveJumpToggle = new AnimatorParameter("ShockwaveJump");
        private readonly AnimatorParameter diveToggle = new AnimatorParameter("Dive");

        private readonly AnimatorParameter idleToggle = new AnimatorParameter("Idle");

        [SerializeField]
        private float dampTime = 0.1f;

        [SerializeField]
        private float aimingExitTime = 3f;

        private Animator characterAnimator;

        private int speedXParameterId;
        private int speedZParameterId;
        private int pitchParameterId;
        private int reloadTriggerId;
        private int shootTriggerId;
        private int aimingToggleId;
        private int stunnedToggleId;

        private IAnimationSpeedProvider speedProvider;
        private IDisposable aimingExitAction;

        private void Start()
        {
            speedXParameterId = Animator.StringToHash(SpeedXParameter);
            speedZParameterId = Animator.StringToHash(SpeedZParameter);
            pitchParameterId = Animator.StringToHash(PitchParameter);

            reloadTriggerId = Animator.StringToHash(ReloadTrigger);
            shootTriggerId = Animator.StringToHash(ShootTrigger);
            aimingToggleId = Animator.StringToHash(AimingToggle);
            stunnedToggleId = Animator.StringToHash(StunnedToggle);
        }

        private void Update()
        {
            characterAnimator.SetFloat(speedXParameterId, speedProvider.CurrentRightSpeed, dampTime, Time.deltaTime);
            characterAnimator.SetFloat(speedZParameterId, speedProvider.CurrentForwardSpeed, dampTime, Time.deltaTime);
        }

        [SuppressMessage(
            "StyleCop.CSharp.OrderingRules",
            "SA1202:ElementsMustBeOrderedByAccess",
            Justification = "Reviewed.")]
        public void SetPitchValue(float value)
        {
            characterAnimator.SetFloat(pitchParameterId, value, dampTime, Time.deltaTime);
        }

        public void Setup(IAnimationSpeedProvider speedProvider, Animator animator)
        {
            this.speedProvider = speedProvider;
            characterAnimator = animator;
        }

        [AnimationUtilityButton(true)]
        public void Reload()
        {
            characterAnimator.SetTrigger(reloadTriggerId);
        }

        [AnimationUtilityButton(true)]
        public void Shoot()
        {
            characterAnimator.SetTrigger(shootTriggerId);

            if (aimingExitAction != null)
            {
                aimingExitAction.Dispose();
                aimingExitAction = null;
            }

            Aim(true);

            aimingExitAction = Scheduler.MainThreadEndOfFrame.Schedule(
                TimeSpan.FromSeconds(aimingExitTime),
                () => Aim(false));
        }

        /// <summary>
        /// Plays stunned animation.
        /// </summary>
        /// <param name="value">Stunned if set to <c>true</c> [value].</param>
        [AnimationUtilityButton(false)]
        public void Stunned(bool value)
        {
            characterAnimator.SetBool(stunnedToggleId, value);
        }

        public void Aim(bool value)
        {
            if (characterAnimator != null)
            {
                characterAnimator.SetBool(aimingToggleId, value);
            }
        }

        [AnimationUtilityButton(false)]
        public void GrenadeAim(bool aim)
        {
            characterAnimator.SetBool(aimGrenadeToggle, aim);
        }

        [AnimationUtilityButton(true)]
        public void GrenadeThrow()
        {
            characterAnimator.SetTrigger(throwGrenadeToggle);
        }

        [AnimationUtilityButton(true)]
        public void Jump()
        {
            characterAnimator.SetTrigger(jumpToggle);
        }

        [AnimationUtilityButton(true)]
        public void Land()
        {
            characterAnimator.SetTrigger(landToggle);
        }

        [AnimationUtilityButton(false)]
        public void StealthDashSetActive(bool isActive)
        {
            characterAnimator.SetBool(stealthDashBool, isActive);
        }

        [AnimationUtilityButton(true)]
        public void ShowWave()
        {
            characterAnimator.SetTrigger(shockwaveToggle);
        }

        [AnimationUtilityButton(true)]
        public void ShockWaveJump()
        {
            characterAnimator.SetTrigger(shockwaveJumpToggle);
        }

        [AnimationUtilityButton(true)]
        public void Dive()
        {
            characterAnimator.SetTrigger(diveToggle);
        }

        public void Idle()
        {
            characterAnimator.SetTrigger(idleToggle);
        }

        [Serializable]
        private class AnimatorParameter
        {
            private string name;
            private int hash;

            public string Name => name;

            public int Hash
            {
                get
                {
                    if (hash == 0)
                    {
                        BakeHash();
                    }

                    return hash;
                }
            }

            public AnimatorParameter(string name)
            {
                this.name = name;
            }

            public void BakeHash()
            {
                hash = Animator.StringToHash(Name);
            }

            public static implicit operator int(AnimatorParameter p)
            {
                return p.Hash;
            }
        }
    }
}