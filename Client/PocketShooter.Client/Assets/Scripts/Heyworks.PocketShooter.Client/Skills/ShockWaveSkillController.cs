using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities.Skills;
using Heyworks.PocketShooter.Realtime.Simulation;
using UniRx;

namespace Heyworks.PocketShooter.Skills
{
    public class ShockWaveSkillController : SkillController
    {
        private readonly AoESkill shockWave;

        private AoESkillInfo AoESkillConfig => (AoESkillInfo)Model.Info;

        public ShockWaveSkillController(OwnedSkill skillModel, SkillSpec spec, LocalCharacter character)
            : base(skillModel, spec, character)
        {
            shockWave = new AoESkill(SkillName.ShockWave, character, AoESkillConfig.AoE);

            character.ModelEvents.SkillCastChanged.Where(
                e => e.SkillName == SkillName)
                .Subscribe(CastChanged)
                .AddTo(character);
        }

        private void CastChanged(SkillCastChangedEvent e)
        {
            if (!e.IsCasting)
            {
                shockWave.Execute();
            }
        }
    }
}
