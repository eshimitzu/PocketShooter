using Heyworks.PocketShooter.Audio;
using Heyworks.PocketShooter.Core.SchedulerManager;
using Heyworks.PocketShooter.Modules.EffectsManager;
using Heyworks.PocketShooter.Networking.Actors;
using UniRx;
using UnityEngine;

namespace Heyworks.PocketShooter.Client
{
    public class RootEffectController : EffectController
    {
        [SerializeField]
        private GameObject trapPrefab;

        [SerializeField]
        private float trapAppearingDelay;

        private GameObject teathEffect;
        private GameObject trap;

        private SchedulerTask trapAppearingDelayTask;

        public override void Setup(NetworkCharacter character)
        {
            base.Setup(character);

            character.ModelEvents.RootChanged.Subscribe(UpdateRoot).AddTo(this);
        }

        public override void Stop()
        {
            Clean();
        }

        private void UpdateRoot(bool isRooted)
        {
            Clean();

            if (isRooted)
            {
                Character.CharacterCommon.AudioController.PostEvent(AudioKeys.Event.PlayTrap);

                teathEffect = EffectsManager.Instance.PlayEffect(EffectType.RageTeeth, transform, true);

                trapAppearingDelayTask = SchedulerManager.Instance.CallActionWithDelay(this, () =>
                {
                    trap = Instantiate(trapPrefab, transform);
                }, trapAppearingDelay);
            }
        }

        private void Clean()
        {
            if (teathEffect != null)
            {
                EffectsManager.Instance.StopEffect(teathEffect);
            }

            if (trapAppearingDelayTask != null)
            {
                SchedulerManager.Instance.RemoveSchedulerTask(trapAppearingDelayTask);
            }

            if (trap != null)
            {
                Destroy(trap);
            }
        }
    }
}