using System;
using Heyworks.PocketShooter.Meta.Data;

namespace Heyworks.PocketShooter.Meta.Entities
{
    [Serializable]
    public class TrooperIdentity : IContentIdentity, IHasGradeAndLevel
    {
        private TrooperIdentity()
        {
        }

        public TrooperIdentity(TrooperClass trooperClass, Grade grade, int level)
        {
            Class = trooperClass;
            Grade = grade;
            Level = level;
        }

        public TrooperClass Class { get; private set; }

        public Grade Grade { get; private set; }

        public int Level { get; private set; }

        public TrooperState ToState() => new TrooperState { Class = Class, CurrentWeapon = null, Grade = Grade, Level = Level };
    }
}
