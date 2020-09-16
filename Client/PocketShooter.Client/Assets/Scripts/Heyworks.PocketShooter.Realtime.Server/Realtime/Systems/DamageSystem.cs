using System;
using Collections.Pooled;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public class DamageSystem : ServerInitiatorSystem
    {
        private readonly GameArmorInfo gameArmorInfo;

        public DamageSystem(GameArmorInfo gameArmorInfo)
        {
            this.gameArmorInfo = gameArmorInfo;
        }

        protected override void Execute(ServerPlayer localPlayer, IServerGame game)
        {
            if (localPlayer.IsDead)
            {
                localPlayer.Damages.Clear();
                return;
            }

            ref ArmorComponent armor = ref localPlayer.Armor;
            ref HealthComponent health = ref localPlayer.Health;

            var damagesCopy = new PooledList<DamageInfo>(localPlayer.Damages.Span);
            var span = damagesCopy.Span;
            localPlayer.Damages.Clear();

            for (int i = 0; i < span.Length; i++)
            {
                ref var damageInfo = ref span[i];
                var damage = damageInfo.Damage;
                float actualDamage;

                switch (damageInfo.DamageType)
                {
                    case DamageType.Pure:
                    case DamageType.Extra:
                        var pureDamage = Math.Min(damage, health.Health);
                        health.Health -= pureDamage;
                        actualDamage = pureDamage;
                        break;
                    default:
                        actualDamage = DamageExtensions.ApplyDamage(ref armor, ref health, damage, gameArmorInfo);
                        break;
                }

                if (localPlayer.IsDead && localPlayer.Effects.Lucky.IsLucky)
                {
                    health.Health = 1f;
                    actualDamage -= 1f;
                }

                var newDamageInfo = new DamageInfo(
                    damageInfo.AttackerId,
                    damageInfo.DamageSource,
                    damageInfo.DamageType,
                    actualDamage);

                localPlayer.Damages.Add(newDamageInfo);

                if (localPlayer.IsDead)
                {
                    localPlayer.ServerEvents.LastKiller = damageInfo.AttackerId;
                    game.MatchResult.PlayerStats[localPlayer.Id].Deaths++;
                    game.MatchResult.PlayerStats[damageInfo.AttackerId].Kills++;
                    break;
                }
            }

            if (localPlayer.Damages.Count > 0)
            {
                localPlayer.InvisibleExpire.ExpireAt = 0;
            }
        }
    }
}