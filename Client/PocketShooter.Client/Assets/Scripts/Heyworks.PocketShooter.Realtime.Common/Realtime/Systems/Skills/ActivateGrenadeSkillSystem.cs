using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Entities.Skills;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public class ActivateGrenadeSkillSystem : ActivateSkillSystem
    {
        public ActivateGrenadeSkillSystem(ITicker ticker, SkillName skillName)
            : base(ticker, skillName)
        {
        }

        protected override bool Execute(OwnedPlayer initiator, OwnedSkill playerSkill, IGame game)
        {
            if (initiator.CanUseSkill(playerSkill.Skill.Name))
            {
                bool isExecutable = base.Execute(initiator, playerSkill, game);

                if (isExecutable)
                {
                    if (playerSkill.Skill is IConsumableSkillForSystem consumableSkill)
                    {
                        consumableSkill.UseCount++;
                        game.ConsumablesState.PlayerStats[initiator.Id].UseOffensive();
                    }
                }

                return isExecutable;
            }

            return false;
        }
    }
}