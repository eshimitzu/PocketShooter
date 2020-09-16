using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Entities.Skills;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public class ActivateLuckySkillSystem : ActivateSkillSystem
    {
        public ActivateLuckySkillSystem(ITicker ticker, SkillName skillName)
            : base(ticker, skillName)
        {
        }

        protected override bool Execute(OwnedPlayer initiator, OwnedSkill playerSkill, IGame game)
        {
            bool isExecutable = base.Execute(initiator, playerSkill, game);

            if (isExecutable)
            {
                initiator.Effects.Lucky.IsLucky = true;
                initiator.LuckyExpire.ExpireAt = Ticker.Current + playerSkill.Info.ActiveTime;
            }

            return isExecutable;
        }
    }
}