using System;
using Heyworks.PocketShooter.Realtime.Entities;

namespace Heyworks.PocketShooter.Realtime.Data
{
    public static class DamageExtensions
    {
        public static bool CannotBeDamaged(this PlayerBase player, TeamNo attackerTeam) =>
            player == null || player.IsDead || player.Effects.Immortal.IsImmortal || attackerTeam == player.Team;

        public static float WeaponShotDamage(this WeaponInfo weaponInfo, bool isHeadshot) =>
            (weaponInfo.Damage / weaponInfo.Fraction) * (isHeadshot ? weaponInfo.CriticalMultiplier : 1);

        public static float ArmorDamage(in this ArmorComponent self, float damage, GameArmorInfo gameArmorInfo) =>
            Math.Min(self.Armor, damage * gameArmorInfo.Impact * gameArmorInfo.DamageFactor);

        public static float HealthDamage(
            in this HealthComponent self,
            float damage,
            float armorDamage,
            GameArmorInfo gameArmorInfo) => Math.Min(self.Health, damage - armorDamage / gameArmorInfo.DamageFactor);

        public static (float armorDamage, float healthDamage) Damage(
            in ArmorComponent armor,
            in HealthComponent health,
            float damage,
            GameArmorInfo gameArmorInfo)
        {
            var armorDamage = armor.ArmorDamage(damage, gameArmorInfo);
            var healthDamage = health.HealthDamage(damage, armorDamage, gameArmorInfo);
            return (armorDamage, healthDamage);
        }

        public static float ApplyDamage(
            ref ArmorComponent armor,
            ref HealthComponent health,
            float damage,
            GameArmorInfo gameArmorInfo)
        {
            var (armorDamage, healthDamage) = Damage(armor, health, damage, gameArmorInfo);
            armor.Armor -= armorDamage;
            health.Health -= healthDamage;
            return armorDamage + healthDamage;
        }
    }
}