using System;
using Heyworks.PocketShooter.Meta.Data;

namespace Heyworks.PocketShooter.Meta.Entities
{
    [Serializable]
    public class WeaponIdentity : IContentIdentity, IHasGradeAndLevel
    {
        private WeaponIdentity()
        {
        }

        public WeaponIdentity(WeaponName name, Grade grade, int level)
        {
            Name = name;
            Grade = grade;
            Level = level;
        }

        public WeaponName Name { get; private set; }

        public Grade Grade { get; private set; }

        public int Level { get; private set; }

        public WeaponState ToState() => new WeaponState { Name = Name, Grade = Grade, Level = Level };
    }
}
