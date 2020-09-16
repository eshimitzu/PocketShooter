using Heyworks.PocketShooter.Audio;
using Heyworks.PocketShooter.Modules.EffectsManager;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Entities.Skills;
using UnityEngine;

namespace Heyworks.PocketShooter.Skills
{
    public class LifestealSkillVisualizer : SkillVisualizer
    {
        private GameObject lifestealEffectCircle;
        private GameObject lifestealEffectSkulls;

        private Vector3 zoneScale;

        private new LifestealSkillSpec Spec { get; }

        private LifestealSkill SkillModel { get; }

        public LifestealSkillVisualizer(LifestealSkill skillModel, LifestealSkillSpec spec, NetworkCharacter character)
            : base(skillModel, spec, character)
        {
            Spec = spec;
            SkillModel = skillModel;
        }

        public override void Visualize()
        {
            zoneScale = new Vector3(SkillModel.Radius, SkillModel.Radius, SkillModel.Radius);

            EffectsManager.Instance.PlayEffect(EffectType.LifestealPuh, Character.transform, true);

            lifestealEffectCircle = EffectsManager.Instance.PlayEffect(EffectType.LifestealCircle, Character.transform);
            lifestealEffectCircle.transform.localScale = zoneScale;

            lifestealEffectSkulls = EffectsManager.Instance.PlayEffect(EffectType.LifestealSkulls, Character.transform, Spec.LifestealEffectSkullsPosition);
            ParticleSystem lifestealEffectSkullsParticleSystem = lifestealEffectSkulls.GetComponent<ParticleSystem>();
            var lifestealEffectSkullsParticleSystemMain = lifestealEffectSkullsParticleSystem.main;
            lifestealEffectSkullsParticleSystemMain.startLifetime = zoneScale.x / Mathf.Abs(lifestealEffectSkullsParticleSystemMain.startSpeed.constant);
            var lifestealEffectSkullsParticleSystemShape = lifestealEffectSkullsParticleSystem.shape;
            lifestealEffectSkullsParticleSystemShape.radius = zoneScale.x;

            Character.CharacterCommon.AudioController.PostEvent(AudioKeys.Event.PlayLifesteal);
        }

        public override void Finish()
        {
            StopVisualize();
        }

        public override void Cancel()
        {
            StopVisualize();
        }

        private void StopVisualize()
        {
            Character.CharacterCommon.AudioController.PostEvent(AudioKeys.Event.StopLifesteal);

            EffectsManager.Instance.StopEffect(lifestealEffectCircle);
            EffectsManager.Instance.StopEffect(lifestealEffectSkulls);
        }
    }
}