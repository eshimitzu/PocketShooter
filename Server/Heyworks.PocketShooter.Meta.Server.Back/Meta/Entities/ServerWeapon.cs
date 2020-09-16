using System;
using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Data;
using Heyworks.PocketShooter.Realtime.Configuration.Data;
using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Meta.Entities
{
    public class ServerWeapon : WeaponBase
    {
        private readonly IWeaponConfiguration weaponConfiguration;

        public ServerWeapon(WeaponState weaponState, IWeaponConfiguration weaponConfiguration)
            : base(weaponState, weaponConfiguration)
        {
            this.weaponConfiguration = weaponConfiguration;
        }

        public WeaponInfo GetWeaponInfo()
        {
            WeaponInfo CreateWeaponInfo(WeaponStatsConfig wsc)
            {
                return new WeaponInfo
                {
                    Name = Name,
                    AttackInterval = Realtime.Constants.ToTicks(wsc.AttackIntervalMs),
                    ClipSize = wsc.ClipSize,
                    MaxRange = wsc.MaxRange,
                    ReloadTime = Realtime.Constants.ToTicks(wsc.ReloadTimeMs),
                    Dispersion = wsc.Dispersion,
                    CriticalMultiplier = wsc.CriticalMultiplier,
                    Damage = wsc.Damage,
                    FractionDispersion = wsc.FractionDispersion,
                    Fraction = wsc.Fraction,
                    AutoAim = wsc.AutoAim,
                };
            }

            var gradeRealtimeStats = weaponConfiguration.GetGradeRealtimeStats(Name, Grade);
            var levelRealtimeStats = weaponConfiguration.GetLevelRealtimeStats(Name, Level);

            var totalRealtimeStats = gradeRealtimeStats.Sum(levelRealtimeStats);

            switch (totalRealtimeStats)
            {
                case MeleeWeaponStatsConfig mwsc:
                    return new MeleeWeaponInfo(CreateWeaponInfo(mwsc), mwsc.HitZoneWidth, mwsc.HitZoneHeight);
                case WarmingUpWeaponStatsConfig wwsc:
                    return new WarmingUpWeaponInfo(
                        CreateWeaponInfo(wwsc), wwsc.WarmingSpeed, wwsc.CoolingSpeed, wwsc.ResetProgressOnShot);
                case WeaponStatsConfig wsc:
                    return CreateWeaponInfo(wsc);
                default:
                    throw new NotImplementedException($"The type of realtime weapon stats {totalRealtimeStats.GetType().Name} is not supported");
            }
        }
    }
}
