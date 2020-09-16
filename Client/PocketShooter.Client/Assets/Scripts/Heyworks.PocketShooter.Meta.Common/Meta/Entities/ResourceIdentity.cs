using System;

namespace Heyworks.PocketShooter.Meta.Entities
{
    [Serializable]
    public class ResourceIdentity : IContentIdentity
    {
        private ResourceIdentity()
        {
        }

        public ResourceIdentity(int cash, int gold)
        {
            Cash = cash;
            Gold = gold;
        }

        public int Cash { get; private set; }

        public int Gold { get; private set; }
    }
}
