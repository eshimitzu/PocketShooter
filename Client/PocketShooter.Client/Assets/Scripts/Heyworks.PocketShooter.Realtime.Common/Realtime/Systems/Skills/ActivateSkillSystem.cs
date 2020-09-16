using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Entities.Skills;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public class ActivateSkillSystem : IOwnerGameSystem
    {
        public ActivateSkillSystem(ITicker ticker, SkillName skillName)
        {
            this.Ticker = ticker;
            this.SkillName = skillName;
        }

        public SkillName SkillName { get; }

        public virtual bool Execute(OwnedPlayer initiator, IGame game)
        {
            var skill = initiator.GetFirstOwnedSkill(SkillName);
            return Execute(initiator, skill, game);
        }

        protected ITicker Ticker { get; }

        protected virtual bool Execute(OwnedPlayer initiator, OwnedSkill playerSkill, IGame game)
        {
            if (playerSkill.Skill is ICooldownSkillForSystem skill
                && skill.State == SkillState.Default
                && initiator.CanUseSkill(skill.Name))
            {
                initiator.InvisibleExpire.ExpireAt = 0;

                if (playerSkill.Info.ActiveTime == 0)
                {
                    skill.State = SkillState.Reloading;
                    skill.StateExpireAt = Ticker.Current + playerSkill.Info.CooldownTime;
                }
                else
                {
                    skill.State = SkillState.Active;
                    skill.StateExpireAt = Ticker.Current + playerSkill.Info.ActiveTime;
                }

                return true;
            }

            return false;
        }
    }
}