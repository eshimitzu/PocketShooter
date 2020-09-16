using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Entities.Skills;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public class ActivateInvisibleSkillSystem : ActivateSkillSystem
    {
        public ActivateInvisibleSkillSystem(ITicker ticker, SkillName skillName)
            : base(ticker, skillName)
        {
        }

        protected override bool Execute(OwnedPlayer initiator, OwnedSkill playerSkill, IGame game)
        {
            bool isExecutable = base.Execute(initiator, playerSkill, game);

            if (isExecutable)
            {
                initiator.Effects.Invisible.IsInvisible = true;
                initiator.InvisibleExpire.ExpireAt = Ticker.Current + playerSkill.Info.ActiveTime;
            }

            return isExecutable;
        }
    }
}