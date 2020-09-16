using System;

namespace Heyworks.PocketShooter.Meta.Entities
{
    [Serializable]
    public class OffensiveIdentity : IContentIdentity, ICountable
    {
        private OffensiveIdentity()
        {
        }

        public OffensiveIdentity(OffensiveName name, int count)
        {
            Name = name;
            Count = count;
        }

        public OffensiveName Name { get; private set; }

        public int Count { get; private set; }
    }
}