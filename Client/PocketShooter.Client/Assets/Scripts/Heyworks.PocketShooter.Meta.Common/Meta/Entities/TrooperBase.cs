using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Heyworks.PocketShooter.Meta.Entities
{
    public abstract class TrooperBase : ArmyItem
    {
        private readonly TrooperState trooperState;
        private readonly ITrooperConfigurationBase trooperConfiguration;

        protected TrooperBase(TrooperState trooperState, ITrooperConfigurationBase trooperConfiguration) 
            : base(trooperState.Id)
        {
            this.trooperState = trooperState;
            this.trooperConfiguration = trooperConfiguration;
        }

        /// <summary>
        /// Gets a trooper's class.
        /// </summary>
        public TrooperClass Class => trooperState.Class;

        /// <summary>
        /// Gets a current trooper's grade.
        /// </summary>
        public override Grade Grade
        {
            get => trooperState.Grade;
            protected set => trooperState.Grade = value;
        }

        /// <summary>
        /// Gets a current trooper's level.
        /// </summary>
        public override int Level
        {
            get => trooperState.Level;
            protected set => trooperState.Level = value;
        }

        /// <summary>
        /// Gets a max trooper's level for the current grade.
        /// </summary>
        public int MaxLevel => trooperConfiguration.GetMaxLevel(Grade);

        public IReadOnlyList<Skill> Skills =>
            trooperConfiguration
            .GetSkills(Class)
            .Select(item => new Skill(item, Grade))
            .ToList();

        public ItemStats Stats =>
            ItemStats.Sum(
                trooperConfiguration.GetGradeStats(Class, Grade),
                trooperConfiguration.GetLevelStats(Class, Level));

        public bool IsMaxLevel => Level >= MaxLevel;

        public override bool IsFirstLevelOnGrade => Level == trooperConfiguration.GetFirstLevel(Grade);

        public int MaxPotentialPower => trooperConfiguration.GetMaxPotentialPower(Class);

        public Price InstantLevelUpPrice =>
            CanLevelUp() ? trooperConfiguration.GetInstantLevelUpPrice(Class, Level + 1) : Price.Zero;

        public Price RegularLevelUpPrice =>
            CanLevelUp() ? trooperConfiguration.GetRegularLevelUpPrice(Class, Level + 1) : Price.Zero;

        public override TimeSpan RegularLevelUpDuration =>
            CanLevelUp() ? trooperConfiguration.GetRegularLevelUpDuration(Class, Level + 1) : TimeSpan.Zero;

        public Price InstantGradeUpPrice =>
            CanGradeUp() ? trooperConfiguration.GetInstantGradeUpPrice(Class, Grade + 1) : Price.Zero;

        public override bool CanLevelUp() => !IsMaxLevel;

        public override void LevelUp()
        {
            if (!CanLevelUp())
            {
                throw new InvalidOperationException("Can't up trooper's level now.");
            }

            Level++;
        }

        public override bool CanGradeUp() => IsMaxLevel && !Grade.IsMax();

        public override void GradeUp()
        {
            if (!CanGradeUp())
            {
                throw new InvalidOperationException("Can't up trooper's grade now.");
            }

            Grade++;
        }
    }
}
