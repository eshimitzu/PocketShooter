using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Heyworks.PocketShooter.Realtime
{
    public class CyclicIdPoolTests
    {
        [Fact]
        public void NotImmediateReuse()
        {
            var pool = new CyclicIdPool(1, 5);
            pool.Acquire(out var one);
            pool.Acquire(out var two);
            pool.Release(one);
            pool.Acquire(out var three);
            Assert.NotEqual(one, three);
            Assert.NotEqual(one, two);
        }

    }
}
