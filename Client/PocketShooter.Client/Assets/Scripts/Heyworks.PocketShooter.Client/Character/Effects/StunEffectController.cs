using Heyworks.PocketShooter.Modules.EffectsManager;
using Heyworks.PocketShooter.Networking.Actors;
using UniRx;
using UnityEngine;

namespace Heyworks.PocketShooter.Client
{
    public class StunEffectController : EffectController
    {
        [SerializeField]
        private Transform stunStarsEffectPoint;

        private GameObject stunStarsEffect;

        public override void Setup(NetworkCharacter character)
        {
            base.Setup(character);

            character.ModelEvents.StunChanged.Subscribe(UpdateStun).AddTo(this);
        }

        public override void Stop()
        {
            Clean();
        }

        private void UpdateStun(bool isStunned)
        {
            Clean();

            if (isStunned)
            {
                Character.CharacterCommon.AnimationController.Stunned(true);
                
                Character.CharacterCommon.AudioController.HandleStun(true);

                stunStarsEffect = EffectsManager.Instance.PlayEffect(EffectType.StunStars, stunStarsEffectPoint);
            }
        }

        private void Clean()
        {
            Character.CharacterCommon.AnimationController.Stunned(false);

            Character.CharacterCommon.AudioController.HandleStun(false);

            EffectsManager.Instance.StopEffect(stunStarsEffect);

            EffectsManager.Instance.PlayEffect(EffectType.StunStarsEnd, stunStarsEffectPoint, true);
        }
    }
}