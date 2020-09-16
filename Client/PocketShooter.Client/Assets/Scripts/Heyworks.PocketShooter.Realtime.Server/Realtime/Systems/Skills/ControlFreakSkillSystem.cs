using System.Linq;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Entities.Skills;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public class ControlFreakSkillSystem : SkillSystem<ISkillForSystem, ControlFreakSkillInfo>
    {
        protected override SkillName SkillName => SkillName.ControlFreak;

        public ControlFreakSkillSystem(ITicker ticker)
            : base(ticker)
        {
        }

        protected override void Execute(ISkillForSystem skill, ControlFreakSkillInfo skillInfo, ServerPlayer initiator, IServerGame game)
        {
            if (initiator.CurrentWeapon.State != WeaponState.Attacking)
            {
                return;
            }

            if (initiator.Shots.Count == 0)
            {
                return;
            }

            foreach (var shotsGroup in initiator.Shots.GroupBy(s => s.AttackedId))
            {
                var victim = game.GetServerPlayer(shotsGroup.Key);

                if (victim.CannotBeDamaged(initiator.Team) || !IsUnderControlEffect(victim))
                {
                    continue;
                }

                var damage = victim.Damages.FindAll(di => di.AttackerId == initiator.Id);

                foreach (var damageInfo in damage)
                {
                    AddExtraDamage(damageInfo.Damage, victim, skill, initiator.Id, skillInfo);
                }
            }
        }

        private static void AddExtraDamage(
            float initialDamage,
            ServerPlayer victim,
            ISkillForSystem skill,
            EntityId initiator,
            ControlFreakSkillInfo config)
        {
            float extraDamage = initialDamage * config.IncreaseDamagePercent / 100f;

            victim.Damages.Add(
                new DamageInfo(
                    initiator,
                    new EntityRef(EntityType.Skill, skill.Id.Item1),
                    DamageType.Extra,
                    extraDamage));
        }

        private bool IsUnderControlEffect(ServerPlayer player)
        {
            return player.Effects.Root.IsRooted || player.Effects.Stun.IsStunned;
        }
    }
}
