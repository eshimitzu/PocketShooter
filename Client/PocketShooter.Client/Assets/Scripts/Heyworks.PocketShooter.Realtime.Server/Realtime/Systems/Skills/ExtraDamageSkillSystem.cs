using System.Collections.Generic;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Entities.Skills;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public class ExtraDamageSkillSystem : SkillSystem<ICooldownSkillForSystem, ExtraDamageSkillInfo>
    {
        private readonly HashSet<ServerPlayer> victims = new HashSet<ServerPlayer>();

        protected override SkillName SkillName => SkillName.ExtraDamage;

        public ExtraDamageSkillSystem(ITicker ticker)
            : base(ticker)
        {
        }

        protected override void Execute(ICooldownSkillForSystem skill, ExtraDamageSkillInfo skillInfo, ServerPlayer initiator, IServerGame game)
        {
            if (skill.State != SkillState.Default || initiator.CurrentWeapon.State != WeaponState.Attacking)
            {
                return;
            }

            foreach (var initiatorShot in initiator.Shots)
            {
                var victim = game.GetServerPlayer(initiatorShot.AttackedId);

                if (victim.CannotBeDamaged(initiator.Team))
                {
                    continue;
                }

                victims.Add(victim);
            }

            foreach (ServerPlayer victim in victims)
            {
                float extraDamage = victim.Health.MaxHealth * skillInfo.DamagePercentOfMaxHealth / 100f;

                victim.Damages.Add(
                    new DamageInfo(
                        initiator.Id,
                        new EntityRef(EntityType.Skill, skill.Id.Item1),
                        DamageType.Extra,
                        extraDamage));

                skill.State = SkillState.Reloading;
                skill.StateExpireAt = Ticker.Current + skillInfo.CooldownTime;
            }

            victims.Clear();
        }
    }
}