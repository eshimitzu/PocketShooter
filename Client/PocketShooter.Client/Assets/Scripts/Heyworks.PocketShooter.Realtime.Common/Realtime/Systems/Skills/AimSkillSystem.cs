using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Entities.Skills;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public sealed class AimSkillSystem : OwnerSystem
    {
        private readonly ITicker ticker;
        private readonly SkillName skillName;
        private readonly bool aiming;

        public AimSkillSystem(ITicker ticker, SkillName skillName, bool aiming)
        {
            this.ticker = ticker;
            this.skillName = skillName;
            this.aiming = aiming;
        }

        public override bool Execute(OwnedPlayer initiator)
        {
            if (initiator.IsStunned || initiator.IsDead)
            {
                return false;
            }

            return
                Execute(initiator, initiator.Skill1.Skill) |
                Execute(initiator, initiator.Skill2.Skill) |
                Execute(initiator, initiator.Skill3.Skill) |
                Execute(initiator, initiator.Skill4.Skill) |
                Execute(initiator, initiator.Skill5.Skill);
        }

        private bool Execute(OwnedPlayer player, Skill playerSkill)
        {
            if (playerSkill.Name == skillName
                && playerSkill is IAimingSkillForSystem skill
                && playerSkill.CanUseSkill())
            {
                skill.Aiming = aiming;

                return true;
            }

            return false;
        }
    }
}