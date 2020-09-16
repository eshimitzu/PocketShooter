using System.Diagnostics.CodeAnalysis;
using Heyworks.PocketShooter.Audio;
using Heyworks.PocketShooter.Character;
using Heyworks.PocketShooter.Modules.EffectsManager;
using Heyworks.PocketShooter.Networking.Actors;
using UnityEngine;

namespace Heyworks.PocketShooter.Client
{
    public class SprintEffectController : EffectController
    {
        [SerializeField]
        private Transform sprintEffectPoint;

        private GameObject effect;

        private PocketCharacterController pocketCharacterController;

        [SuppressMessage( "StyleCop.CSharp.OrderingRules",  "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed.")]
        private void Update()
        {
            if (pocketCharacterController != null &&
                effect != null && effect.activeSelf)
            {
                float movementAmount = new Vector2(pocketCharacterController.CurrentForwardSpeed, pocketCharacterController.CurrentRightSpeed).magnitude;

                AudioController.Instance.SetRTPC(AudioKeys.RTPC.SprintActive, movementAmount, gameObject);
            }
        }

        public override void Setup(NetworkCharacter character)
        {
            base.Setup(character);

            pocketCharacterController = Character.GetComponent<PocketCharacterController>();
        }

        public void Play()
        {
            Clean();

            effect = EffectsManager.Instance.PlayEffect(EffectType.Sprint, sprintEffectPoint);

            Character.CharacterCommon.AudioController.HandleRunSprint();
        }

        public override void Stop()
        {
            Clean();
        }

        private void Clean()
        {
            if (effect != null && effect.activeSelf)
            {
                EffectsManager.Instance.StopEffect(effect);

                Character.CharacterCommon.AudioController.HandleStopSprint();
            }
        }
    }
}