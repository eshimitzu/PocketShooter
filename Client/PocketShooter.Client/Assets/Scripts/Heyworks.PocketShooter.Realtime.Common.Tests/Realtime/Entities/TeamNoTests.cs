using Heyworks.PocketShooter.Realtime.Entities;
using NUnit.Framework;

namespace Heyworks.PocketShooter.Realtime.Entites
{
    [TestFixture]
    public class TeamNoTests
    {
        [Test]
        public void TeamNoOrderTest()
        {
            Assert.AreEqual((byte)TeamNo.None, 0);
            Assert.AreEqual((byte)TeamNo.First, 1);
            Assert.AreEqual((byte)TeamNo.Second, 2);
        }
    }
}