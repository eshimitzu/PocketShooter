using System;

namespace Heyworks.PocketShooter.Meta.Entities
{
    public abstract class ArmyItem
    {
        protected ArmyItem(int id)
        {
            Id = id;
        }

        public int Id { get; }

        public abstract Grade Grade { get; protected set; }

        public abstract int Level { get; protected set; }

        public abstract bool IsFirstLevelOnGrade { get; }

        public abstract bool CanLevelUp();

        public abstract void LevelUp();

        public abstract bool CanGradeUp();

        public abstract void GradeUp();

        public abstract TimeSpan RegularLevelUpDuration { get; }
    }
}
