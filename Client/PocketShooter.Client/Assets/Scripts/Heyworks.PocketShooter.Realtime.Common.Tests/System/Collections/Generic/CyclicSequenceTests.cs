using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Heyworks.PocketShooter.Realtime
{
    public class CyclicSequenceTests
    {
        [Test]
        public void InsertLoop()
        {
            var buffer = new SafeCyclicSequence(32);

            var random = new Random();

            for (uint i = 0; i < 100; i++)
            {
                var frame = random.Next(0, 32);

                var frameIndex = buffer.Insert(frame);

                Assert.AreEqual(frameIndex, frame);
            }
        }

        [Test]
        public void Contains()
        {
            var sut = new SafeCyclicSequence(32);
            sut.Insert(123);
            Assert.False(sut.ContainsKey(42));
            Assert.False(sut.ContainsKey(13));
            Assert.False(sut.ContainsKey(122));
            Assert.False(sut.ContainsKey(124));
            Assert.False(sut.ContainsKey(666));
        }

        [Test]
        public void Contains2()
        {
            var sut = new SafeCyclicSequence(32);
            sut.Insert(123);
            sut.Insert(125);
            Assert.True(sut.ContainsKey(125));
            Assert.False(sut.ContainsKey(124));
            Assert.True(sut.ContainsKey(125));
        }

        [Test]
        public void CurrentNext()
        {
            var sut = new SafeCyclicSequence(32);            
            Assert.AreEqual(0, sut.NextSequenceNumber);
            Assert.AreEqual(-1, sut.LastSequenceNumber);
            sut.Insert(123);

            Assert.AreEqual(124, sut.NextSequenceNumber);
            Assert.AreEqual(123, sut.LastSequenceNumber);

            sut.Insert(120);
            Assert.AreEqual(124, sut.NextSequenceNumber);
            Assert.AreEqual(123, sut.LastSequenceNumber);
        }        

        [Test]
        public void TryInsert()
        {
            var sut = new SafeCyclicSequence(32);            
            sut.Insert(123);
            sut.Insert(123);
            sut.Insert(1);            
        }             
    }
}
