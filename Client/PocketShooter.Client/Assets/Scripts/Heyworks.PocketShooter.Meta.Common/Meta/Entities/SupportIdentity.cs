using System;

namespace Heyworks.PocketShooter.Meta.Entities
{
    [Serializable]
    public class SupportIdentity : IContentIdentity, ICountable
    {
        private SupportIdentity()
        {
        }

        public SupportIdentity(SupportName name, int count)
        {
            Name = name;
            Count = count;
        }

        public SupportName Name { get; private set; }

        public int Count { get; private set; }
    }
}