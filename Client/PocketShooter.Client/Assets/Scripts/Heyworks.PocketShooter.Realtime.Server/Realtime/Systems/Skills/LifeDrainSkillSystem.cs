using System;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Entities.Skills;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public class LifeDrainSkillSystem : SkillSystem<ISkillForSystem, LifeDrainSkillInfo>
    {
        private readonly GameArmorInfo gameArmorInfo;

        protected override SkillName SkillName => SkillName.LifeDrain;

        public LifeDrainSkillSystem(ITicker ticker, GameArmorInfo gameArmorInfo)
            : base(ticker)
        {
            this.gameArmorInfo = gameArmorInfo;
        }

        protected override void Execute(ISkillForSystem skill, LifeDrainSkillInfo skillInfo, ServerPlayer initiator, IServerGame game)
        {
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
                var (armorDamage, healthDamage) = DamageExtensions.Damage(victim.Armor, victim.Health, damage, gameArmorInfo);
                var drained = (float)Math.Ceiling((armorDamage + healthDamage) * skillInfo.DrainPercent);
                initiator.Heals.Add(new HealInfo(HealType.LifeDrain, drained));
            }
        }
    }
}