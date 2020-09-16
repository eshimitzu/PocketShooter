using System;
using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Data;

namespace Heyworks.PocketShooter.Meta.Entities
{
    public abstract class HelmetBase : ArmyItem
    {
        private readonly HelmetState helmetState;
        private readonly IHelmetConfigurationBase helmetConfiguration;

        protected HelmetBase(HelmetState helmetState, IHelmetConfigurationBase helmetConfiguration)
            : base(helmetState.Id)
        {
            this.helmetState = helmetState;
            this.helmetConfiguration = helmetConfiguration;
        }

        public HelmetName Name => helmetState.Name;

        public override Grade Grade
        {
            get => helmetState.Grade;
            protected set => helmetState.Grade = value;
        }

        public override int Level
        {
            get => helmetState.Level;
            protected set => helmetState.Level = value;
        }

        public ItemStats Stats =>
            ItemStats.Sum(
                helmetConfiguration.GetGradeStats(Name, Grade),
                helmetConfiguration.GetLevelStats(Name, Level));

        public int MaxLevel => helmetConfiguration.GetMaxLevel(Grade);

        public bool IsMaxLevel => Level >= MaxLevel;

        public override bool IsFirstLevelOnGrade => Level == helmetConfiguration.GetFirstLevel(Grade);

        public int MaxPotentialPower => helmetConfiguration.GetMaxPotentialPower(Name);

        public Price InstantLevelUpPrice =>
            CanLevelUp() ? helmetConfiguration.GetInstantLevelUpPrice(Name, Level + 1) : Price.Zero;

        public Price RegularLevelUpPrice =>
            CanLevelUp() ? helmetConfiguration.GetRegularLevelUpPrice(Name, Level + 1) : Price.Zero;

        public override TimeSpan RegularLevelUpDuration =>
            CanLevelUp() ? helmetConfiguration.GetRegularLevelUpDuration(Name, Level + 1) : TimeSpan.Zero;

        public Price InstantGradeUpPrice =>
            CanGradeUp() ? helmetConfiguration.GetInstantGradeUpPrice(Name, Grade + 1) : Price.Zero;

        public override bool CanLevelUp() => !IsMaxLevel;

        public override void LevelUp()
        {
            if (!CanLevelUp())
            {
                throw new InvalidOperationException("Can't up helmet's level now.");
            }

            Level++;
        }

        public override bool CanGradeUp() => IsMaxLevel && !Grade.IsMax();

        public override void GradeUp()
        {
            if (!CanGradeUp())
            {
                throw new InvalidOperationException("Can't up helmet's grade now.");
            }

            Grade++;
        }
    }
}