using System;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Entities.Skills;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public sealed class SkillExecutionSystem : OwnerSystem
    {
        private readonly ITicker ticker;

        public SkillExecutionSystem(ITicker ticker)
        {
            this.ticker = ticker;
        }

        public override bool Execute(OwnedPlayer initiator)
        {
            Execute(initiator.Skill1);
            Execute(initiator.Skill2);
            Execute(initiator.Skill3);
            Execute(initiator.Skill4);
            Execute(initiator.Skill5);
            return true;
        }

        public bool Execute(OwnedSkill playerSkill)
        {
            if (playerSkill.Skill is ICooldownSkillForSystem skill && ticker.Current >= skill.StateExpireAt)
            {
                switch (skill.State)
                {
                    case SkillState.Active:
                        Deactivate(skill, playerSkill.Info);
                        return true;
                    case SkillState.Reloading:
                        Reload(skill);
                        return true;
                    case SkillState.Default:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(skill.State), skill.State, null);
                }
            }

            return false;
        }

        private void Deactivate(ICooldownSkillForSystem skill, SkillInfo skillInfo)
        {
            skill.State = SkillState.Reloading;
            skill.StateExpireAt += skillInfo.CooldownTime - skillInfo.ActiveTime;
        }

        private void Reload(ICooldownSkillForSystem skill)
        {
            skill.State = SkillState.Default;
            skill.StateExpireAt = int.MaxValue;
        }
    }
}