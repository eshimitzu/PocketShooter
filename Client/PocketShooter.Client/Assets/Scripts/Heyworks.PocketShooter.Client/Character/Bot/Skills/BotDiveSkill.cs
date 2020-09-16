using BehaviorDesigner.Runtime.Tasks;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities.Skills;
using Heyworks.PocketShooter.Realtime.Simulation;
using Heyworks.PocketShooter.Skills;
using UniRx;

namespace Heyworks.PocketShooter.Character.Bot.Skills
{
    public class BotDiveSkill : BotSkill
    {
        private DiveSkillInfo DiveSkillConfig => (DiveSkillInfo)Model.Info;

        public BotDiveSkill(OwnedSkill skillModel, SkillSpec spec, BotCharacter bot)
            : base(skillModel, spec, bot)
        {
            Bot.ModelEvents.SkillCastChanged.Where(
                    e => e.SkillName == SkillName)
                .Subscribe(CastChanged)
                .AddTo(Bot);
        }

        protected override void CastChanged(SkillCastChangedEvent e)
        {
            base.CastChanged(e);

            if (!e.IsCasting)
            {
                var dive = new BotAoESkill(SkillName.Dive, Bot, DiveSkillConfig.AoE);
                dive.Execute();
            }
        }
    }
}
