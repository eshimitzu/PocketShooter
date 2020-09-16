using Heyworks.PocketShooter.Audio;
using Heyworks.PocketShooter.Modules.EffectsManager;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Entities.Skills;
using Heyworks.PocketShooter.Realtime.Simulation;
using Heyworks.PocketShooter.Skills.Configuration;
using UniRx;
using UnityEngine.Assertions;

namespace Heyworks.PocketShooter.Skills
{
    public class ShockWaveJumpSkillVisualizer : SkillVisualizer
    {
        private readonly ShockWaveJumpSkillSpec spec;

        public ShockWaveJumpSkillVisualizer(Skill skillModel, ShockWaveJumpSkillSpec spec, NetworkCharacter character)
            : base(skillModel, spec, character)
        {
            Assert.IsNotNull(spec);

            this.spec = spec;

            character.ModelEvents.SkillCastChanged.Where(
                    e => e.SkillName == SkillName)
                .Subscribe(CastChanged)
                .AddTo(character);
        }

        public override void Visualize()
        {
            base.Visualize();

            Character.CharacterCommon.AnimationController.ShockWaveJump();
        }

        private void CastChanged(SkillCastChangedEvent e)
        {
            if (!e.IsCasting)
            {
                // TODO: setup effect size
                EffectsManager.Instance.PlayEffect(EffectType.ShockWave, Character.transform.position, true);
                Character.CharacterCommon.AudioController.PostEvent(AudioKeys.Event.PlayShockwave);
            }
        }
    }
}
