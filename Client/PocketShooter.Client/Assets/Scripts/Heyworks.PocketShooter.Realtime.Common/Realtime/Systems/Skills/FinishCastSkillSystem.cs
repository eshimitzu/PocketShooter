using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Entities.Skills;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public class FinishCastSkillSystem : OwnerSystem
    {
        private readonly ITicker ticker;

        public FinishCastSkillSystem(ITicker ticker)
        {
            this.ticker = ticker;
        }

        public override bool Execute(OwnedPlayer player)
        {
            Execute(player.Skill1);
            Execute(player.Skill2);
            Execute(player.Skill3);
            Execute(player.Skill4);
            Execute(player.Skill5);
            return true;
        }

        public bool Execute(OwnedSkill ownedSkill)
        {
            if (ownedSkill.Skill is ICastableSkillForSystem skill && skill.Casting && ticker.Current >= skill.CastingExpireAt)
            {
                skill.Casting = false;
                skill.CastingExpireAt = int.MaxValue;
                return true;
            }

            return false;
        }
    }
}