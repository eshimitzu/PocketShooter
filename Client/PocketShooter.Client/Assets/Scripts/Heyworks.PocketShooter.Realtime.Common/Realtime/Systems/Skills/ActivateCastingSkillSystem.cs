using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Entities.Skills;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public class ActivateCastingSkillSystem : ActivateSkillSystem
    {
        public ActivateCastingSkillSystem(ITicker ticker, SkillName skillName)
            : base(ticker, skillName)
        {
        }

        protected override bool Execute(OwnedPlayer initiator, OwnedSkill playerSkill, IGame game)
        {
            bool isExecutable = base.Execute(initiator, playerSkill, game);

            if (isExecutable)
            {
                var skill = (ICastableSkillForSystem)playerSkill.Skill;

                skill.Casting = true;
                skill.CastingExpireAt = Ticker.Current + ((AoESkillInfo)playerSkill.Info).CastingTime;
            }

            return isExecutable;
        }
    }
}
