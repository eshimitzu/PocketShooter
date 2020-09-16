using System;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public interface IConnectionListener
    {
        void Disconnected(Type client);

        void Connected(Type client);
    }
}