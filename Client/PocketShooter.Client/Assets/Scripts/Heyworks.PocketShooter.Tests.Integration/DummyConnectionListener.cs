using System;
using Heyworks.PocketShooter.Meta.Communication;

namespace Heyworks.PocketShooter.Tests
{
    public class DummyConnectionListener : IConnectionListener
    {
        public void Disconnected(Type client)
        {
        }

        public void Connected(Type client)
        {
        }
    }
}