using System.Collections.Generic;
using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.TestUtils
{
    public static class InfoUtils
    {
        public static TrooperInfo TestTrooperInfo => new TrooperInfo(new TrooperMetaInfo
        {
            Class = TrooperClass.Rambo,
            Grade = Grade.Uncommon,
            Level = 1,
            MaxLevel = 10,
            Health = 2000,
            Armor = 500,
            Movement = 5.0,
            Power = 100,
            MaxPotentialPower = 1000,
            ItemsInfo = new ItemsMetaInfo(WeaponName.M16A4, 10, HelmetName.Helmet2, 20, ArmorName.Armor3, 30),
        })
        {
            MaxArmor = 500,
            MaxHealth = 2000,
            Skill1 = new SkillInfo { ActiveTime = 1000, CooldownTime = 5000, Name = SkillName.StunningHit },
            Skill2 = new SkillInfo { ActiveTime = 1000, CooldownTime = 5000, Name = SkillName.StunningHit },
            Skill3 = new SkillInfo { ActiveTime = 1000, CooldownTime = 5000, Name = SkillName.StunningHit },
            Skill4 = new SkillInfo { ActiveTime = 1000, CooldownTime = 5000, Name = SkillName.StunningHit },
            Skill5 = new SkillInfo { ActiveTime = 1000, CooldownTime = 5000, Name = SkillName.StunningHit },
            Speed = 5,
            Weapon = new WeaponInfo
            {
                AttackInterval = 100,
                AutoAim = true,
                ClipSize = 10,
                CriticalMultiplier = 2,
                Damage = 100,
                Dispersion = 1,
                Fraction = 1,
                FractionDispersion = 1,
                MaxRange = 100,
                Name = WeaponName.M16A4,
                ReloadTime = 1000
            }
        };

        public static TeamInfo TestTeamInfo(TeamNo number) =>
            new TeamInfo(number, new[] { new SpawnPointInfo(0, 0, 0, 0) });

        public static DominationModeInfo
            TestDominationModeInfo(DominationZoneInfo[] dominationZonesInfo) => new DominationModeInfo(
            10,
            1000,
            8,
            1,
            0,
            1000000,
            new GameArmorInfo(0.5f, 1),
            new DominationMapInfo(
                new[] { TestTeamInfo(TeamNo.First), TestTeamInfo(TeamNo.Second) },
                dominationZonesInfo ?? new[] { new DominationZoneInfo(0, 0, 0, 0, 1), }));

    }
}