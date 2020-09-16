using System;
using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Data;

namespace Heyworks.PocketShooter.Meta.Entities
{
    public abstract class ArmorBase : ArmyItem
    {
        private readonly ArmorState armorState;
        private readonly IArmorConfigurationBase armorConfiguration;

        protected ArmorBase(ArmorState armorState, IArmorConfigurationBase armorConfiguration):
            base(armorState.Id)
        {
            this.armorState = armorState;
            this.armorConfiguration = armorConfiguration;
        }

        public ArmorName Name => armorState.Name;

        public override Grade Grade
        {
            get => armorState.Grade;
            protected set => armorState.Grade = value;
        }

        public override int Level
        {
            get => armorState.Level;
            protected set => armorState.Level = value;
        }

        public ItemStats Stats =>
            ItemStats.Sum(
                armorConfiguration.GetGradeStats(Name, Grade),
                armorConfiguration.GetLevelStats(Name, Level));

        public int MaxLevel => armorConfiguration.GetMaxLevel(Grade);

        public bool IsMaxLevel => Level >= MaxLevel;

        public override bool IsFirstLevelOnGrade => Level == armorConfiguration.GetFirstLevel(Grade);

        public int MaxPotentialPower => armorConfiguration.GetMaxPotentialPower(Name);

        public Price InstantLevelUpPrice =>
            CanLevelUp() ? armorConfiguration.GetInstantLevelUpPrice(Name, Level + 1) : Price.Zero;

        public Price RegularLevelUpPrice =>
            CanLevelUp() ? armorConfiguration.GetRegularLevelUpPrice(Name, Level + 1) : Price.Zero;

        public override TimeSpan RegularLevelUpDuration =>
            CanLevelUp() ? armorConfiguration.GetRegularLevelUpDuration(Name, Level + 1) : TimeSpan.Zero;

        public Price InstantGradeUpPrice =>
            CanGradeUp() ? armorConfiguration.GetInstantGradeUpPrice(Name, Grade + 1) : Price.Zero;

        public override bool CanLevelUp() => !IsMaxLevel;

        public override void LevelUp()
        {
            if (!CanLevelUp())
            {
                throw new InvalidOperationException("Can't up armor's level now.");
            }

            Level++;
        }

        public override bool CanGradeUp() => IsMaxLevel && !Grade.IsMax();

        public override void GradeUp()
        {
            if (!CanGradeUp())
            {
                throw new InvalidOperationException("Can't up armor's grade now.");
            }

            Grade++;
        }
    }
}