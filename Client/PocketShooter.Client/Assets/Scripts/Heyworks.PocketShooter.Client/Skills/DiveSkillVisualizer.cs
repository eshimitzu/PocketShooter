using Heyworks.PocketShooter.Audio;
using Heyworks.PocketShooter.Modules.EffectsManager;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Entities.Skills;
using Heyworks.PocketShooter.Realtime.Simulation;
using UniRx;
using UnityEngine;
using Heyworks.PocketShooter.Configuration;

namespace Heyworks.PocketShooter.Skills
{
    public class DiveSkillVisualizer : SkillVisualizer
    {
        private DiveSkillSpec spec;

        private GameObject diveButton;
        private GameObject rocket;

        public DiveSkillVisualizer(Skill skillModel, DiveSkillSpec spec, NetworkCharacter character)
            : base(skillModel, spec, character)
        {
            this.spec = spec;

            character.ModelEvents.SkillCastChanged.Where(
                    e => e.SkillName == SkillName)
                .Subscribe(CastChanged)
                .AddTo(character);

            Character.CharacterCommon.TrooperAvatar.AnimationEventsHandler.OnDiveButtonPressed += AnimationEventsHandler_OnDiveButtonPressed;
            Character.CharacterCommon.TrooperAvatar.AnimationEventsHandler.OnDiveButtonFinished += AnimationEventsHandler_OnDiveButtonFinished;
        }

        public override void Visualize()
        {
            base.Visualize();

            Clean();

            Character.CharacterCommon.AnimationController.Dive();

            EffectsManager.Instance.PlayEffect(EffectType.DiveAim, Character.transform, true);

            Character.CharacterView.SetWeaponViewVisible(false);

            Transform diveButtonParent = Character.CharacterCommon.TrooperAvatar.WeaponViewParent;
            diveButton = Object.Instantiate(spec.DiveButtonPrefab, diveButtonParent);
            diveButton.transform.localPosition = spec.DiveButtonPosition;
            diveButton.transform.localRotation = Quaternion.Euler(spec.DiveButtonRotation.x, spec.DiveButtonRotation.y, spec.DiveButtonRotation.z);

            Character.CharacterCommon.AudioController.PostEvent(AudioKeys.Event.PlayDiveAim);
        }

        public override void Cancel()
        {
            base.Cancel();

            Clean();
        }

        public override void Finish()
        {
            base.Finish();

            Clean();
        }

        private void Clean()
        {
            ResetWeapon();
        }

        private void ResetWeapon()
        {
            if (diveButton != null)
            {
                Object.Destroy(diveButton);
            }

            Character.CharacterView.SetWeaponViewVisible(true);
        }

        private void CastChanged(SkillCastChangedEvent e)
        {
            if (!e.IsCasting)
            {
                EffectsManager.Instance.PlayEffect(EffectType.DiveExplosion, Character.transform.position, true);

                Character.CharacterCommon.AudioController.PostEvent(AudioKeys.Event.PlayDiveExplosion);
            }
        }

        private void AnimationEventsHandler_OnDiveButtonPressed()
        {
            Character.CharacterCommon.AudioController.PostEvent(AudioKeys.Event.PlayDive);
            Character.CharacterCommon.AudioController.PostEvent(AudioKeys.Event.PlayDiveRocket);

            rocket = Object.Instantiate(spec.RocketPrefab);
            rocket.transform.position = Character.transform.TransformPoint(spec.RocketPosition);

            rocket.GetComponent<DiveRocket>().MoveToPoint(Character.transform, spec.RocketFlyingTime);
        }

        private void AnimationEventsHandler_OnDiveButtonFinished()
        {
            ResetWeapon();
        }
    }
}
