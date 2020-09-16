using System;
using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Data;

namespace Heyworks.PocketShooter.Meta.Entities
{
    public abstract class WeaponBase : ArmyItem
    {
        private readonly WeaponState weaponState;
        private readonly IWeaponConfigurationBase weaponConfiguration;

        protected WeaponBase(WeaponState weaponState, IWeaponConfigurationBase weaponConfiguration)
        : base(weaponState.Id)
        {
            this.weaponState = weaponState;
            this.weaponConfiguration = weaponConfiguration;
        }

        public WeaponName Name => weaponState.Name;

        public override Grade Grade
        {
            get => weaponState.Grade;
            protected set => weaponState.Grade = value;
        }

        public override int Level
        {
            get => weaponState.Level;
            protected set => weaponState.Level = value;
        }

        /// <summary>
        /// Gets the weapon stats.
        /// </summary>
        public ItemStats Stats =>
            ItemStats.Sum(
                weaponConfiguration.GetGradeStats(Name, Grade),
                weaponConfiguration.GetLevelStats(Name, Level));

        public int MaxLevel => weaponConfiguration.GetMaxLevel(Grade);

        public bool IsMaxLevel => Level >= MaxLevel;

        public override bool IsFirstLevelOnGrade => Level == weaponConfiguration.GetFirstLevel(Grade);

        public int MaxPotentialPower => weaponConfiguration.GetMaxPotentialPower(Name);

        public Price InstantLevelUpPrice =>
            CanLevelUp() ? weaponConfiguration.GetInstantLevelUpPrice(Name, Level + 1) : Price.Zero;

        public Price RegularLevelUpPrice =>
            CanLevelUp() ? weaponConfiguration.GetRegularLevelUpPrice(Name, Level + 1): Price.Zero;

        public override TimeSpan RegularLevelUpDuration =>
            CanLevelUp() ? weaponConfiguration.GetRegularLevelUpDuration(Name, Level + 1) : TimeSpan.Zero;

        public Price InstantGradeUpPrice =>
            CanGradeUp() ? weaponConfiguration.GetInstantGradeUpPrice(Name, Grade + 1) : Price.Zero;

        public override bool CanLevelUp() => !IsMaxLevel;

        public override void LevelUp()
        {
            if (!CanLevelUp())
            {
                throw new InvalidOperationException("Can't up weapon's level now.");
            }

            Level++;
        }

        public override bool CanGradeUp() => IsMaxLevel && !Grade.IsMax();

        public override void GradeUp()
        {
            if (!CanGradeUp())
            {
                throw new InvalidOperationException("Can't up weapon's grade now.");
            }

            Grade++;
        }
    }
}
