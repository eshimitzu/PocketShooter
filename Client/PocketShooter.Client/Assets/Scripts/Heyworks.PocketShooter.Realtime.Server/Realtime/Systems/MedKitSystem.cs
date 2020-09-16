using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public class MedKitSystem : ServerInitiatorSystem
    {
        private readonly ITicker ticks;

        public MedKitSystem(ITicker ticks)
        {
            this.ticks = ticks;
        }

        protected override void Execute(ServerPlayer initiator, IServerGame game)
        {
            if (initiator.Effects.MedKit.IsHealing && initiator.MedKit.NextHealAt <= ticks.Current)
            {
                var medKitSkillInfo = initiator.GetFirstSkillInfo<MedKitSkillInfo>(SkillName.MedKit);
                var toHeal = medKitSkillInfo.HealingPercent * initiator.Health.MaxHealth;
                initiator.Heals.Add(new HealInfo(HealType.MedKit, toHeal));

                initiator.MedKit.NextHealAt += medKitSkillInfo.HealInterval;
            }
        }
    }
}