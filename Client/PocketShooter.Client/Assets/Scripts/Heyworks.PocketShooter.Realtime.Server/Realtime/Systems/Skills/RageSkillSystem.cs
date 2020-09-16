using System;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Entities.Skills;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public class RageSkillSystem : SkillSystem<ICooldownSkillForSystem, RageSkillInfo>
    {
        protected override SkillName SkillName => SkillName.Rage;

        public RageSkillSystem(ITicker ticker)
            : base(ticker)
        {
        }

        protected override void Execute(ICooldownSkillForSystem skill, RageSkillInfo skillInfo, ServerPlayer initiator, IServerGame game)
        {
            if (initiator.CurrentWeapon.State == WeaponState.Attacking ||
                initiator.RageExpire.LastWeaponState == WeaponState.Attacking)
            {
                if (Ticker.Current >= initiator.RageExpire.IncreaseDamageAt)
                {
                    initiator.RageExpire.IncreaseDamageAt = Ticker.Current + skillInfo.IncreaseDamageInterval;
                    initiator.Effects.Rage.AdditionalDamagePercent = Math.Min(initiator.Effects.Rage.AdditionalDamagePercent + skillInfo.IncreaseDamagePercent, 100);
                }

                initiator.RageExpire.DecreaseDamageAt = Ticker.Current + skillInfo.DecreaseDamageInterval;
            }
            else
            {
                if (Ticker.Current >= initiator.RageExpire.DecreaseDamageAt)
                {
                    initiator.RageExpire.DecreaseDamageAt = Ticker.Current + skillInfo.DecreaseDamageInterval;
                    initiator.Effects.Rage.AdditionalDamagePercent = Math.Max(initiator.Effects.Rage.AdditionalDamagePercent - skillInfo.DecreaseDamagePercent, 0);
                }

                initiator.RageExpire.IncreaseDamageAt = Ticker.Current + skillInfo.IncreaseDamageInterval;
            }

            initiator.RageExpire.LastWeaponState = initiator.CurrentWeapon.State;
        }
    }
}