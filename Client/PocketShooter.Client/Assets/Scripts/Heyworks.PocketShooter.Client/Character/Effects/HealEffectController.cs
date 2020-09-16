using Heyworks.PocketShooter.Modules.EffectsManager;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Simulation;
using UniRx;
using UnityEngine;

namespace Heyworks.PocketShooter.Client
{
    public class HealEffectController : EffectController
    {
        [SerializeField]
        private Transform healEffectPoint;

        private GameObject healEffect;
        private GameObject regenerationEffect;

        public override void Setup(NetworkCharacter character)
        {
            base.Setup(character);

            character.ModelEvents.MedKitUsed.Subscribe(StarHealEffect).AddTo(this);
            character.ModelEvents.HealChanged.Subscribe(StarHealEffect).AddTo(this);
            character.ModelEvents.Healed.Subscribe(ProcessHeal).AddTo(this);
        }

        public override void Stop()
        {
            EffectsManager.Instance.StopEffect(healEffect);
            EffectsManager.Instance.StopEffect(regenerationEffect);
        }

        private void PlayHeal()
        {
            Character.CharacterCommon.AudioController.HandleHeal();
            healEffect = EffectsManager.Instance.PlayEffect(EffectType.Heal, healEffectPoint, true);
        }

        private void PlayRegeneration()
        {
            Character.CharacterCommon.AudioController.HandleRegeneration();
            regenerationEffect = EffectsManager.Instance.PlayEffect(EffectType.Regeneration, transform, true);
        }

        private void ProcessHeal(HealingServerEvent healEvent)
        {
            var heals = healEvent.Heals.Span;

            for (var i = 0; i < heals.Length; i++)
            {
                ref readonly var heal = ref heals[i];

                if (healEffect == null || !healEffect.activeSelf)
                {
                    if (heal.Type == HealType.LifeDrain ||
                        heal.Type == HealType.Lifesteal)
                    {
                        PlayHeal();
                    }
                }

                if (regenerationEffect == null || !regenerationEffect.activeSelf)
                {
                    if (heal.Type == HealType.RegenerationOnKill)
                    {
                        PlayRegeneration();

                        break;
                    }
                }
            }
        }

        private void StarHealEffect(bool healed)
        {
            if (healed)
            {
                if (healEffect == null || !healEffect.activeSelf)
                {
                    PlayHeal();
                }
            }
        }
    }
}