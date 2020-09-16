using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Entities.Skills;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public class ActivateHealSkillSystem : ActivateSkillSystem
    {
        public ActivateHealSkillSystem(ITicker ticker, SkillName skillName)
            : base(ticker, skillName)
        {
        }

        protected override bool Execute(OwnedPlayer initiator, OwnedSkill playerSkill, IGame game)
        {
            bool isExecutable = base.Execute(initiator, playerSkill, game);

            if (isExecutable)
            {
                initiator.Effects.Heal.IsHealing = true;

                initiator.Heal.NextHealAt = Ticker.Current;
                initiator.Heal.ExpiredAt = Ticker.Current + ((HealSkillInfo)playerSkill.Info).RegenerationInterval;
            }

            return isExecutable;
        }
    }
}