using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Entities.Skills;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public class ActivateLifestealSkillSystem : ActivateSkillSystem
    {
        public ActivateLifestealSkillSystem(ITicker ticker, SkillName skillSkillName)
            : base(ticker, skillSkillName)
        {
        }

        protected override bool Execute(OwnedPlayer initiator, OwnedSkill playerSkill, IGame game)
        {
            bool isExecutable = base.Execute(initiator, playerSkill, game);
            {
                var skillInfo = (LifestealSkillInfo)playerSkill.Info;
                var skill = (LifestealSkill)playerSkill.Skill;
                skill.NextStealTime = Ticker.Current + skillInfo.StealPeriod;
                skill.Radius = skillInfo.AoE.Radius;
            }

            return isExecutable;
        }
    }
}