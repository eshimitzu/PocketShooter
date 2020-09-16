using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Heyworks.PocketShooter.System.Collections.Generic
{
    public class CyclicSequenceBufferTests
    {
        [Test]
        public void Insert_Test()
        {
            var buffer = new SafeCyclicSequenceBuffer<int>(32);
            buffer.Insert(123, 13);
            Assert.Throws<InvalidOperationException>(() => buffer.Insert(1, 13));
            Assert.Throws<InvalidOperationException>(() => buffer.Insert(123-32, 13));
            buffer.Insert(123 - 31, 42);
        }

        [Test]
        public void Contains_Test()
        {
            var buffer = new SafeCyclicSequenceBuffer<int>(32);
            Assert.False(buffer.ContainsKey(100000));
            buffer.Insert(123, 13);
            Assert.True(buffer.ContainsKey(123));
            buffer.Insert(123 - 31, 42);
            Assert.True(buffer.ContainsKey(123 - 31));

            Assert.False(buffer.ContainsKey(12));
            Assert.False(buffer.ContainsKey(123-30));
            Assert.False(buffer.ContainsKey(123-1));
            Assert.False(buffer.ContainsKey(124));
        }

        [Test]
        public void Slides()
        {
            var buffer = new SafeCyclicSequenceBuffer<int>(32);
            buffer.Insert(123, 13);
            Assert.True(buffer.ContainsKey(123));
            buffer.Insert(123+33, 42);
            Assert.False(buffer.ContainsKey(123));
        }

        [Test]
        public void CanInsert_Test()
        {
            var buffer = new SafeCyclicSequenceBuffer<int>(32);
            Assert.True(buffer.CanInsert(100000));
            buffer.Insert(123, 13);
            Assert.False(buffer.IsStale(123));
            Assert.False(buffer.CanInsert(123));
            buffer.Insert(123 - 31, 42);
            Assert.False(buffer.IsStale(123 - 31));
            Assert.False(buffer.CanInsert(123 - 31));

            Assert.False(buffer.CanInsert(12));
            Assert.True(buffer.IsStale(12));
            Assert.True(buffer.CanInsert(123 - 30));
            Assert.True(buffer.CanInsert(123 - 1));
            Assert.False(buffer.IsStale(124));
            Assert.True(buffer.CanInsert(124));
            Assert.True(buffer.CanInsert(666));
            Assert.True(buffer.CanInsert(100000));
        }

        [Test]
        public void TryInsert_Test()
        {
            var buffer = new SafeCyclicSequenceBuffer<int>(32);
            Assert.True(buffer.TryInsert(123, 13));
            Assert.True(buffer.TryInsert(123-31, 13));
            Assert.False(buffer.TryInsert(123 - 32, 13));
            Assert.False(buffer.TryInsert(42, 13));
            Assert.True(buffer.TryInsert(666, 13));
            Assert.False(buffer.TryInsert(123, 13));
        }
    }
}
