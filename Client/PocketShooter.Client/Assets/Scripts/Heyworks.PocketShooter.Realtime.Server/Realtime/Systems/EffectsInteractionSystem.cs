﻿using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Entities.Skills;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public class EffectsInteractionSystem : ServerInitiatorSystem
    {
        protected override void Execute(ServerPlayer initiator, IServerGame game)
        {
            if (initiator.Effects.Immortal.IsImmortal)
            {
                initiator.Effects.Root.IsRooted = false;
                initiator.Effects.Stun.IsStunned = false;

                for (int i = 0; i < initiator.Damages.Count; i++)
                {
                    DamageInfo damageInfo = initiator.Damages[i];

                    var newDamageInfo = new DamageInfo(
                        damageInfo.AttackerId,
                        damageInfo.DamageSource,
                        damageInfo.DamageType,
                        0);

                    initiator.Damages[i] = newDamageInfo;
                }
            }

            if (initiator.Effects.Jump.IsJumping || initiator.Effects.Dash.IsDashing)
            {
                initiator.Effects.Root.IsRooted = false;
                initiator.Effects.Stun.IsStunned = false;
            }

            if (initiator.IsStunned)
            {
                InterruptSkill(initiator.Skill1);
                InterruptSkill(initiator.Skill2);
                InterruptSkill(initiator.Skill3);
                InterruptSkill(initiator.Skill4);
                InterruptSkill(initiator.Skill5);
            }

            if (initiator.IsRooted)
            {
                InterruptSkillByRoot(initiator.Skill1);
                InterruptSkillByRoot(initiator.Skill1);
                InterruptSkillByRoot(initiator.Skill1);
                InterruptSkillByRoot(initiator.Skill1);
                InterruptSkillByRoot(initiator.Skill1);
            }
        }

        private void InterruptSkill(OwnedSkill ownedSkill)
        {
            if (ownedSkill.Skill is ICastableSkillForSystem castableSkill)
            {
                castableSkill.Casting = false;
            }

            if (ownedSkill.Skill is IAimingSkillForSystem aimingSkill)
            {
                aimingSkill.Aiming = false;
            }
        }

        private void InterruptSkillByRoot(OwnedSkill ownedSkill)
        {
            if (ownedSkill.Skill is ICastableSkillForSystem castableSkill)
            {
                castableSkill.Casting = false;
            }
        }
    }
}