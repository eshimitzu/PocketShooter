using Heyworks.PocketShooter.Audio;
using Heyworks.PocketShooter.Modules.EffectsManager;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Entities.Skills;
using UnityEngine;
using UnityEngine.Assertions;

namespace Heyworks.PocketShooter.Skills
{
    public class ShockWaveSkillVisualizer : SkillVisualizer
    {
        private readonly ShockWaveSkillSpec spec;

        private GameObject airHorn;

        public ShockWaveSkillVisualizer(Skill skillModel, ShockWaveSkillSpec spec, NetworkCharacter character)
            : base(skillModel, spec, character)
        {
            Assert.IsNotNull(spec);

            this.spec = spec;

            Character.CharacterCommon.TrooperAvatar.AnimationEventsHandler.OnShockWave += AnimationEventsHandler_OnShockWave;
            Character.CharacterCommon.TrooperAvatar.AnimationEventsHandler.OnShockWaveFinished += AnimationEventsHandler_OnShockWaveFinished;
        }

        public override void Visualize()
        {
            Clean();

            Character.CharacterView.SetWeaponViewVisible(false);

            Transform airHornParent = Character.CharacterCommon.TrooperAvatar.WeaponViewParent;
            airHorn = Object.Instantiate(spec.AirHornPrefab, airHornParent);
            airHorn.transform.localPosition = spec.AirHornPosition;
            airHorn.transform.localRotation = Quaternion.Euler(spec.AirHornRotation.x, spec.AirHornRotation.y, spec.AirHornRotation.z);

            Character.CharacterCommon.AnimationController.ShowWave();
        }

        public override void Cancel()
        {
            base.Cancel();

            Clean();
        }

        private void Clean()
        {
            if (airHorn != null)
            {
                Object.Destroy(airHorn);
            }

            Character.CharacterView.SetWeaponViewVisible(true);
        }

        private void AnimationEventsHandler_OnShockWave()
        {
            Character.CharacterCommon.AudioController.PostEvent(AudioKeys.Event.PlayShockwave);
            Character.CharacterCommon.AudioController.PostEvent(AudioKeys.Event.PlayShockwaveHorn);

            EffectsManager.Instance.PlayEffect(EffectType.ShockWave, Character.transform, true);
        }

        private void AnimationEventsHandler_OnShockWaveFinished()
        {
            Clean();
        }
    }
}