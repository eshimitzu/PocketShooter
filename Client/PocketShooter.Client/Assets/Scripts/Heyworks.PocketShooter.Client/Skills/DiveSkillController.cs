using Heyworks.PocketShooter.Configuration;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities.Skills;
using Heyworks.PocketShooter.Realtime.Simulation;
using UniRx;

namespace Heyworks.PocketShooter.Skills
{
    public class DiveSkillController : SkillController
    {
        private DiveSkillInfo DiveSkillConfig => (DiveSkillInfo)Model.Info;

        public DiveSkillController(OwnedSkill skillModel, SkillSpec spec, LocalCharacter character)
            : base(skillModel, spec, character)
        {
            character.ModelEvents.SkillCastChanged.Where(
                    e => e.SkillName == SkillName)
                .Subscribe(CastChanged)
                .AddTo(character);
        }

        private void CastChanged(SkillCastChangedEvent e)
        {
            if (!e.IsCasting)
            {
                var dive = new AoESkill(SkillName.Dive, Character, DiveSkillConfig.AoE);
                dive.Execute();
            }
        }
    }
}
