using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Entities.Skills;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public class ActivateStealthSprintSkillSystem : ActivateSkillSystem
    {
        public ActivateStealthSprintSkillSystem(ITicker ticker, SkillName skillName)
            : base(ticker, skillName)
        {
        }

        protected override bool Execute(OwnedPlayer initiator, OwnedSkill playerSkill, IGame game)
        {
            bool isExecutable = base.Execute(initiator, playerSkill, game);

            if (isExecutable)
            {
                var skillInfo = (StealthSprintSkillInfo)playerSkill.Info;
                initiator.Effects.Invisible.IsInvisible = true;
                initiator.InvisibleExpire.ExpireAt = Ticker.Current + skillInfo.StealthActiveTime;
            }

            return isExecutable;
        }
    }
}