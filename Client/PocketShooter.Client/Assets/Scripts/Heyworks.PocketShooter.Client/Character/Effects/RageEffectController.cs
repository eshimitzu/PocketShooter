using Heyworks.PocketShooter.Networking.Actors;
using UniRx;
using UnityEngine;
using Heyworks.PocketShooter.Modules.EffectsManager;
using Heyworks.PocketShooter.Audio;

namespace Heyworks.PocketShooter.Client
{
    public class RageEffectController : EffectController
    {
        [SerializeField]
        private int rageLv2;
        [SerializeField]
        private int rageLv3;
        [SerializeField]
        private int rageLv4;
        [SerializeField]
        private Transform rageEffectPoint;

        private int currentLv = 1;
        private GameObject rageEffect;

        public override void Setup(NetworkCharacter character)
        {
            base.Setup(character);

            character.ModelEvents.RageChanged.Subscribe(UpdateRage).AddTo(this);
        }

        public override void Stop()
        {
            StopEffect(rageEffect);
            EffectsManager.Instance.StopEffect(rageEffect);
        }

        private void UpdateRage(int progress)
        {
            if (progress >= rageLv4)
            {
                if (currentLv != 4)
                {
                    currentLv = 4;
                    StopEffect(rageEffect);
                    rageEffect = EffectsManager.Instance.PlayEffect(EffectType.RageLv4, rageEffectPoint, true);
                    Character.CharacterCommon.AudioController.PostEvent(AudioKeys.Event.PlayRage);
                }
            }
            else if (progress >= rageLv3)
            {
                if (currentLv != 3)
                {
                    currentLv = 3;
                    StopEffect(rageEffect);
                    rageEffect = EffectsManager.Instance.PlayEffect(EffectType.RageLv3, rageEffectPoint, true);
                }
            }
            else if (progress >= rageLv2)
            {
                if (currentLv != 2)
                {
                    currentLv = 2;
                    StopEffect(rageEffect);
                    rageEffect = EffectsManager.Instance.PlayEffect(EffectType.RageLv2, rageEffectPoint, true);
                }
            }
            else if (progress >= 0)
            {
                if (currentLv != 1)
                {
                    currentLv = 1;
                    StopEffect(rageEffect);
                }
            }
        }

        private void StopEffect(GameObject effect)
        {
            if (effect != null)
            {
                effect.GetComponent<ParticleSystem>().Stop(true);
            }
        } 
    }
}