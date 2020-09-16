using System;
using System.Linq;
using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Data;
using Heyworks.PocketShooter.Realtime.Configuration.Data;
using Heyworks.PocketShooter.Realtime.Data;
using RealtimeConstants = Heyworks.PocketShooter.Realtime.Constants;

namespace Heyworks.PocketShooter.Meta.Entities
{
    public class ServerTrooper : TrooperBase
    {
        private readonly ArmyState armyState;
        private readonly TrooperState trooperState;
        private readonly ITrooperConfiguration trooperConfiguration;
        private readonly IWeaponConfiguration weaponConfiguration;
        private readonly IHelmetConfiguration helmetConfiguration;
        private readonly IArmorConfiguration armorConfiguration;
        private readonly ISkillConfiguration skillConfiguration;

        public ServerTrooper(
            ArmyState armyState,
            TrooperState trooperState,
            ITrooperConfiguration trooperConfiguration,
            IWeaponConfiguration weaponConfiguration,
            IHelmetConfiguration helmetConfiguration,
            IArmorConfiguration armorConfiguration,
            ISkillConfiguration skillConfiguration)
            : base(trooperState, trooperConfiguration)
        {
            this.armyState = armyState;
            this.trooperState = trooperState;
            this.trooperConfiguration = trooperConfiguration;
            this.weaponConfiguration = weaponConfiguration;
            this.helmetConfiguration = helmetConfiguration;
            this.armorConfiguration = armorConfiguration;
            this.skillConfiguration = skillConfiguration;
        }

        public ServerWeapon CurrentWeapon
        {
            get
            {
                var weaponState = armyState.Weapons.First(item => item.Name == trooperState.CurrentWeapon);

                return new ServerWeapon(weaponState, weaponConfiguration);
            }
        }

        public ServerHelmet CurrentHelmet
        {
            get
            {
                var helmetState = armyState.Helmets.First(item => item.Name == trooperState.CurrentHelmet);

                return new ServerHelmet(helmetState, helmetConfiguration);
            }
        }

        public ServerArmor CurrentArmor
        {
            get
            {
                var armorState = armyState.Armors.First(item => item.Name == trooperState.CurrentArmor);

                return new ServerArmor(armorState, armorConfiguration);
            }
        }

        public TrooperInfo GetTooperInfo()
        {
            var trooperRealtimeStats = TrooperStatsConfig.Sum(
                trooperConfiguration.GetGradeRealtimeStats(Class, Grade),
                trooperConfiguration.GetLevelRealtimeStats(Class, Level));

            var skills = Skills.Select(item => GetSkillInfo(item)).ToList();

            var itemsMetaInfo = new ItemsMetaInfo(
                CurrentWeapon.Name,
                CurrentWeapon.Stats.Power,
                CurrentHelmet.Name,
                CurrentHelmet.Stats.Power,
                CurrentArmor.Name,
                CurrentArmor.Stats.Power);

            var trooperMetaInfo = new TrooperMetaInfo
            {
                Class = Class,
                Grade = Grade,
                Level = Level,
                MaxLevel = MaxLevel,
                Health = Stats.Health,
                Armor = Stats.Armor,
                Movement = Stats.Movement,
                Power = Stats.Power,
                MaxPotentialPower = MaxPotentialPower,
                ItemsInfo = itemsMetaInfo,
            };

            return new TrooperInfo(trooperMetaInfo)
            {
                MaxHealth = trooperRealtimeStats.MaxHealth + CurrentHelmet.GetMaxHealthInfo(),
                MaxArmor = trooperRealtimeStats.MaxArmor + CurrentArmor.GetMaxArmorInfo(),
                Speed = trooperRealtimeStats.Speed,
                Weapon = CurrentWeapon.GetWeaponInfo(),
                Skill1 = skills[0],
                Skill2 = skills[1],
                Skill3 = skills[2],
                Skill4 = skills[3],
                Skill5 = skills[4],
            };
        }

        private SkillInfo GetSkillInfo(Skill skill)
        {
            SkillInfo CreateSkillInfo(SkillStatsConfig ssc)
            {
                return new SkillInfo
                {
                    Name = skill.Name,
                    CooldownTime = RealtimeConstants.ToTicks(ssc.CooldownTimeMs),
                    ActiveTime = RealtimeConstants.ToTicks(ssc.ActiveTimeMs),
                };
            }

            var stats = skillConfiguration.GetRealtimeStats(skill.Name, skill.Grade);

            switch (stats)
            {
                case DiveSkillStatsConfig dssc:
                    return new DiveSkillInfo(
                        CreateSkillInfo(dssc), RealtimeConstants.ToTicks(dssc.CastingTimeMs), new AoEInfo(dssc.Radius, dssc.Height), dssc.Damage);
                case LifestealSkillStatsConfig lssc:
                    return new LifestealSkillInfo(
                        CreateSkillInfo(lssc), RealtimeConstants.ToTicks(lssc.StealPeriodMs), lssc.StealPercent, new AoEInfo(lssc.Radius, lssc.Height));
                case AoESkillStatsConfig assc:
                    return new AoESkillInfo(
                        CreateSkillInfo(assc), RealtimeConstants.ToTicks(assc.CastingTimeMs), new AoEInfo(assc.Radius, assc.Height));
                case LifeDrainSkillStatsConfig ldssc:
                    return new LifeDrainSkillInfo(CreateSkillInfo(ldssc), ldssc.DrainPercent);
                case ControlFreakSkillStatsConfig cfssc:
                    return new ControlFreakSkillInfo(CreateSkillInfo(cfssc), cfssc.IncreaseDamagePercent);
                case ExtraDamageSkillStatsConfig edssc:
                    return new ExtraDamageSkillInfo(CreateSkillInfo(edssc), edssc.DamagePercentOfMaxHealth);
                case GrenadeSkillStatsConfig gssc:
                    return new GrenadeSkillInfo(
                        CreateSkillInfo(gssc), gssc.SplashDamage, gssc.Damage, gssc.ExplosionRadius, RealtimeConstants.ToTicks(gssc.CastingTimeMs), gssc.AvailablePerSpawn);
                case HealSkillStatsConfig hssc:
                    return new HealSkillInfo(
                        CreateSkillInfo(hssc), hssc.HealingPercent, RealtimeConstants.ToTicks(hssc.HealIntervalMs), hssc.HealIntervalsPerUsage);
                case MedKitSkillStatsConfig mkssc:
                    return new MedKitSkillInfo(
                        CreateSkillInfo(mkssc), mkssc.HealingPercent, RealtimeConstants.ToTicks(mkssc.HealIntervalMs), mkssc.HealIntervalsPerUsage, mkssc.AvailablePerSpawn);
                case RageSkillStatsConfig rssc:
                    return new RageSkillInfo(
                        CreateSkillInfo(rssc), rssc.IncreaseDamagePercent, rssc.DecreaseDamagePercent, RealtimeConstants.ToTicks(rssc.IncreaseDamageIntervalMs), RealtimeConstants.ToTicks(rssc.DecreaseDamageIntervalMs));
                case RegenerationOnKillSkillStatsConfig rokssc:
                    return new RegenerationOnKillSkillInfo(CreateSkillInfo(rokssc), rokssc.RegenerationPercent);
                case StealthSprintSkillStatsConfig ssssc:
                    return new StealthSprintSkillInfo(
                        CreateSkillInfo(ssssc), ssssc.SpeedMultiplier, RealtimeConstants.ToTicks(ssssc.StealthActiveTimeMs));
                case SprintSkillStatsConfig sssc:
                    return new SprintSkillInfo(CreateSkillInfo(sssc), sssc.SpeedMultiplier);
                case StealthDashSkillStatsConfig sdssc:
                    return new StealthDashSkillInfo(
                        CreateSkillInfo(sdssc), RealtimeConstants.ToTicks(sdssc.CastingTimeMs), RealtimeConstants.ToTicks(sdssc.DashTimeMs), sdssc.Length, sdssc.Speed);
                case SkillStatsConfig ssc:
                    return CreateSkillInfo(ssc);
                default:
                    throw new NotImplementedException($"The type of realtime skill stats {stats.GetType().Name} is not supported");
            }
        }
    }
}
