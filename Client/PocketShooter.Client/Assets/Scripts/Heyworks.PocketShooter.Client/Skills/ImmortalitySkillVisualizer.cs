using Heyworks.PocketShooter.Audio;
using Heyworks.PocketShooter.Core.SchedulerManager;
using Heyworks.PocketShooter.Modules.EffectsManager;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Entities.Skills;
using Heyworks.PocketShooter.Realtime.Simulation;
using UniRx;
using UnityEngine;

namespace Heyworks.PocketShooter.Skills
{
    public class ImmortalitySkillVisualizer : SkillVisualizer
    {
        private ImmortalitySkillSpec spec;

        private GameObject currentImmortalyEffect;

        private bool isIdle;

        private SchedulerTask returningToIdleSchedulerTask;

        public ImmortalitySkillVisualizer(Skill skillModel, ImmortalitySkillSpec spec, NetworkCharacter character)
            : base(skillModel, spec, character)
        {
            this.spec = spec;

            character.ModelEvents.Damaged.Subscribe(HandleDamages).AddTo(character);
        }

        public override void Visualize()
        {
            currentImmortalyEffect = EffectsManager.Instance.PlayEffect(EffectType.ImmortalyDefend, Character.transform, spec.ImmortalyEffectPoint);

            Character.CharacterCommon.AudioController.PostEvent(AudioKeys.Event.PlayImmortalitySphere);

            isIdle = false;

            ScheduleIdle();
        }

        public override void Finish()
        {
            Cancel();
        }

        public override void Cancel()
        {
            EffectsManager.Instance.StopEffect(currentImmortalyEffect);

            SchedulerManager.Instance.RemoveSchedulerTask(returningToIdleSchedulerTask);

            Character.CharacterCommon.AudioController.PostEvent(AudioKeys.Event.StopImmortalitySphere);

            isIdle = false;
        }

        private void HandleDamages(DamagedServerEvent dse)
        {
            if (Character.Model.Effects.Immortal.IsImmortal && isIdle)
            {
                isIdle = false;

                EffectsManager.Instance.StopEffect(currentImmortalyEffect);

                currentImmortalyEffect = EffectsManager.Instance.PlayEffect(EffectType.ImmortalyDefend, Character.transform, spec.ImmortalyEffectPoint);

                Character.CharacterCommon.AudioController.PostEvent(AudioKeys.Event.PlayImmortalityImpact);

                ScheduleIdle();
            }
        }

        private void ScheduleIdle()
        {
            returningToIdleSchedulerTask = SchedulerManager.Instance.CallActionWithDelay(Character, () =>
            {
                isIdle = true;

                EffectsManager.Instance.StopEffect(currentImmortalyEffect);

                currentImmortalyEffect = EffectsManager.Instance.PlayEffect(EffectType.ImmortalyIdle, Character.transform, spec.ImmortalyEffectPoint);
            }, spec.ReturingTimeToIdle);
        }
    }
}