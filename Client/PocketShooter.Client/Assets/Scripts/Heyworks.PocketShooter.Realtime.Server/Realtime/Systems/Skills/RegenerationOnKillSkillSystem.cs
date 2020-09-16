using System.Collections.Generic;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Entities.Skills;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public class RegenerationOnKillSkillSystem : SkillSystem<ICooldownSkillForSystem, RegenerationOnKillSkillInfo>
    {
        private readonly Dictionary<ServerPlayer, float> victimTotalDamage = new Dictionary<ServerPlayer, float>();
        private readonly GameArmorInfo gameArmorInfo;

        protected override SkillName SkillName => SkillName.RegenerationOnKill;

        public RegenerationOnKillSkillSystem(ITicker ticker, GameArmorInfo gameArmorInfo)
            : base(ticker)
        {
            this.gameArmorInfo = gameArmorInfo;
        }

        protected override void Execute(ICooldownSkillForSystem skill, RegenerationOnKillSkillInfo skillInfo, ServerPlayer initiator, IServerGame game)
        {
            if (skill.State != SkillState.Default || initiator.CurrentWeapon.State != WeaponState.Attacking)
            {
                return;
            }

            var weaponInfo = initiator.CurrentWeapon.Info;
            var shots = initiator.Shots.Span;
            for (var i = 0; i < shots.Length; i++)
            {
                ref readonly var shot = ref shots[i];
                var victim = game.GetServerPlayer(shot.AttackedId);

                if (victim.CannotBeDamaged(initiator.Team))
                {
                    continue;
                }

                var damage = weaponInfo.WeaponShotDamage(shot.IsHeadshot);
                var (_, healthDamage) = DamageExtensions.Damage(victim.Armor, victim.Health, damage, gameArmorInfo);

                victimTotalDamage.TryGetValue(victim, out damage);
                victimTotalDamage[victim] = damage + healthDamage;
            }

            foreach (var victim in victimTotalDamage)
            {
                if (victim.Value >= victim.Key.Health.Health)
                {
                    var regenAmount = initiator.Health.MaxHealth * skillInfo.RegenerationPercent;
                    initiator.Heals.Add(new HealInfo(HealType.RegenerationOnKill, regenAmount));
                    // marker-damage
                    victim.Key.Damages.Add(
                        new DamageInfo(
                            initiator.Id,
                            new EntityRef(EntityType.RegenerationOnKillSkill, skill.Id.Item1),
                            DamageType.Pure,
                            0f));

                    skill.State = SkillState.Reloading;
                    skill.StateExpireAt = Ticker.Current + skillInfo.CooldownTime;
                }
            }

            victimTotalDamage.Clear();
        }
    }
}