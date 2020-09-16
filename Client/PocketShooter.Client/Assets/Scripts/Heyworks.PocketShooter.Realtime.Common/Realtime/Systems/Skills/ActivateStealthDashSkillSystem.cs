using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Entities.Skills;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public class ActivateStealthDashSkillSystem : ActivateInvisibleSkillSystem
    {
        public ActivateStealthDashSkillSystem(ITicker ticker, SkillName skillName)
            : base(ticker, skillName)
        {
        }

        protected override bool Execute(OwnedPlayer initiator, OwnedSkill playerSkill, IGame game)
        {
            bool isExecutable = base.Execute(initiator, playerSkill, game);

            if (isExecutable)
            {
                var skillInfo = (StealthDashSkillInfo)playerSkill.Info;

                var skill = (ICastableSkillForSystem)playerSkill.Skill;
                skill.Casting = true;
                skill.CastingExpireAt = Ticker.Current + skillInfo.CastingTime;

                initiator.Effects.Dash.IsDashing = true;
                initiator.DashExpire.ExpireAt = Ticker.Current + skillInfo.CastingTime + skillInfo.DashTime;
            }

            return isExecutable;
        }
    }
}