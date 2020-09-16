using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Entities.Skills;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public class ActivateMedKitSkillSystem : ActivateSkillSystem, IOwnerGameSystem
    {
        public ActivateMedKitSkillSystem(ITicker ticker, SkillName skillName)
            : base(ticker, skillName)
        {
        }

        protected override bool Execute(OwnedPlayer initiator, OwnedSkill playerSkill, IGame game)
        {
            if (initiator.CanUseFirstAidKit())
            {
                bool isExecutable = base.Execute(initiator, playerSkill, game);

                if (isExecutable)
                {
                    initiator.Effects.MedKit.IsHealing = true;

                    initiator.MedKit.NextHealAt = Ticker.Current;
                    initiator.MedKit.ExpiredAt = Ticker.Current + ((MedKitSkillInfo)playerSkill.Info).RegenerationInterval;

                    if (playerSkill.Skill is IConsumableSkillForSystem consumableSkill)
                    {
                        consumableSkill.UseCount++;
                        game.ConsumablesState.PlayerStats[initiator.Id].UseSupport();
                    }
                }

                return isExecutable;
            }

            return false;
        }
    }
}