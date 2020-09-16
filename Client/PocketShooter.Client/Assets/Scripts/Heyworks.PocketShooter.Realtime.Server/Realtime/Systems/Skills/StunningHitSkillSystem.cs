using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Entities.Skills;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public class StunningHitSkillSystem : SkillSystem<ICooldownSkillForSystem, SkillInfo>
    {
        protected override SkillName SkillName => SkillName.StunningHit;

        public StunningHitSkillSystem(ITicker ticker)
            : base(ticker)
        {
        }

        protected override void Execute(ICooldownSkillForSystem skill, SkillInfo skillInfo, ServerPlayer initiator, IServerGame game)
        {
            if (skill.State == SkillState.Default &&
                initiator.CurrentWeapon.State == WeaponState.Attacking)
            {
                foreach (var initiatorShot in initiator.Shots)
                {
                    var victim = game.GetServerPlayer(initiatorShot.AttackedId);

                    if (victim.CannotBeDamaged(initiator.Team))
                    {
                        continue;
                    }

                    victim.Effects.Stun.IsStunned = true;
                    victim.StunExpire.ExpireAt = Ticker.Current + skillInfo.ActiveTime;

                    skill.State = SkillState.Reloading;
                    skill.StateExpireAt = Ticker.Current + skillInfo.CooldownTime;
                }
            }
        }
    }
}
