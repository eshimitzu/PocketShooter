using Heyworks.PocketShooter.Modules.EffectsManager;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Entities.Skills;
using Heyworks.PocketShooter.Skills.Configuration;

namespace Heyworks.PocketShooter.Skills
{
    public class InstantReloadSkillVisualizer : SkillVisualizer
    {
        private InstantReloadSkillSpec spec;

        public InstantReloadSkillVisualizer(Skill skillModel, InstantReloadSkillSpec spec, NetworkCharacter character)
            : base(skillModel, spec, character)
        {
            this.spec = spec;
        }

        public override void Visualize()
        {
            EffectsManager.Instance.PlayEffect(EffectType.InstantReload, Character.transform, spec.InstantReloadEffectPosition, true);

            Character.CharacterCommon.AudioController.HandleInstantReload();
        }
    }
}