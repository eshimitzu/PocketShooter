using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public class HealSystem : ServerInitiatorSystem
    {
        private readonly ITicker ticks;

        public HealSystem(ITicker ticks)
        {
            this.ticks = ticks;
        }

        protected override void Execute(ServerPlayer initiator, IServerGame game)
        {
            if (initiator.Effects.Heal.IsHealing && initiator.Heal.NextHealAt <= ticks.Current)
            {
                var healConfig = initiator.GetFirstSkillInfo<HealSkillInfo>(SkillName.Heal);
                var toHeal = healConfig.HealingPercent * initiator.Health.MaxHealth;
                initiator.Heals.Add(new HealInfo(HealType.MedKit, toHeal));

                initiator.Heal.NextHealAt += healConfig.HealInterval;
            }
        }
    }
}