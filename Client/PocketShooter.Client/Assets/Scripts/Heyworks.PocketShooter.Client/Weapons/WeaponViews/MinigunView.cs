using Heyworks.PocketShooter.Audio;
using UnityEngine;

namespace Heyworks.PocketShooter.Weapons
{
    public class MinigunView : WeaponView, IWarmingUpWeaponView
    {
        private float currentProgress;

        [SerializeField]
        private Transform barrelTransform;

        [SerializeField]
        private float maxBarrelSpeed;

        public void UpdateWarmupProgress(float progress)
        {
            AudioController.Instance.SetRTPC(AudioKeys.RTPC.WarmupConsumableWeapon, progress, gameObject);

            if (progress <= Mathf.Epsilon)
            {
                AudioController.Instance.PostEvent(AudioKeys.Event.PlayWarmUpStopMinigun, gameObject);
            }
            else if (currentProgress <= Mathf.Epsilon)
            {
                AudioController.Instance.PostEvent(AudioKeys.Event.PlayWarmUpMinigun, gameObject);
            }

            this.currentProgress = progress;
        }

        private void Update()
        {
            barrelTransform.Rotate(0f, 0f, currentProgress * maxBarrelSpeed * Time.deltaTime);
        }

        private void OnDestroy()
        {
            if (AudioController.Instance)
            {
                AudioController.Instance.PostEvent(AudioKeys.Event.PlayWarmUpStopMinigun, gameObject);
            }
        }
    }
}