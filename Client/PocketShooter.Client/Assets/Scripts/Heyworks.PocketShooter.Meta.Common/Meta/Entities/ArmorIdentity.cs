using System;
using Heyworks.PocketShooter.Meta.Data;

namespace Heyworks.PocketShooter.Meta.Entities
{
    [Serializable]
    public class ArmorIdentity : IContentIdentity, IHasGradeAndLevel
    {
        private ArmorIdentity()
        {
        }

        public ArmorIdentity(ArmorName name, Grade grade, int level)
        {
            Name = name;
            Grade = grade;
            Level = level;
        }

        public ArmorName Name { get; private set; }

        public Grade Grade { get; private set; }

        public int Level { get; private set; }

        public ArmorState ToState() => new ArmorState { Name = Name, Grade = Grade, Level = Level };
    }
}