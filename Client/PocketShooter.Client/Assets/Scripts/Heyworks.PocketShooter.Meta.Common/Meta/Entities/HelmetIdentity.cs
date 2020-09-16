using System;
using Heyworks.PocketShooter.Meta.Data;

namespace Heyworks.PocketShooter.Meta.Entities
{
    [Serializable]
    public class HelmetIdentity : IContentIdentity, IHasGradeAndLevel
    {
        private HelmetIdentity()
        {
        }

        public HelmetIdentity(HelmetName name, Grade grade, int level)
        {
            Name = name;
            Grade = grade;
            Level = level;
        }

        public HelmetName Name { get; private set; }

        public Grade Grade { get; private set; }

        public int Level { get; private set; }

        public HelmetState ToState() => new HelmetState { Name = Name, Grade = Grade, Level = Level };
    }
}