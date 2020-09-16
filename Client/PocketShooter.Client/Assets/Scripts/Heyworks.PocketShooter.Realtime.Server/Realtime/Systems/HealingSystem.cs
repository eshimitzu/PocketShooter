using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public sealed class HealingSystem : ServerInitiatorSystem
    {
        // TODO: v.shimkovich test together with multiple heal sources
        protected override void Execute(ServerPlayer initiator, IServerGame game)
        {
            ref var health = ref initiator.Health;

            var maxHealth = initiator.Health.MaxHealth;
            for (var i = 0; i < initiator.Heals.Count; i++)
            {
                HealInfo healInfo = initiator.Heals[i];
                var rest = maxHealth - health.Health;
                if (rest < healInfo.Amount)
                {
                    if (rest == 0)
                    {
                        initiator.Heals.RemoveRange(i, initiator.Heals.Count - i);
                        break;
                    }
                    else
                    {
                        health.Health = maxHealth;
                        initiator.Heals[i] = new HealInfo(healInfo.Type, rest);
                    }
                }
                else
                {
                    health.Health += healInfo.Amount;
                }
            }
        }
    }
}