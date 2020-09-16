using System;
using System.Linq;
using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Meta.Services;
using Heyworks.PocketShooter.Realtime.Configuration.Data;
using MoreLinq;
using NUnit.Framework;

namespace Heyworks.PocketShooter.Meta.Configuration
{
    [TestFixture]
    public class MatchMakingQueueTests
    {
        public MatchMakingConfiguration matchMaking => new MatchMakingConfiguration();

        [Test]
        public async Task NoPlayersNoDequeue()
        {
            var matchMakingQueue = new MatchMakingQueue();
            Assert.False(matchMakingQueue.Any());
        }

        [Test]
        [Ignore("As of now algorithm is simplier, uncomment as soon as more clever")]
        public async Task OnePlayerLessThanTime()
        {
            var now = DateTime.UtcNow;
            var matchMakingQueue = new MatchMakingQueue();
            matchMakingQueue.Add(new QueuedPlayer(Guid.NewGuid(), now.AddMilliseconds(100_000), 42));
            var result = matchMakingQueue.Pop(10, matchMaking);
            Assert.AreEqual(0, result.players.Count());
        }

        [Test]
        public async Task ManyPlayersLessTimeButMoreThanNeeded()
        {
            var now = DateTime.UtcNow;
            var matchMakingQueue = new MatchMakingQueue();
            Enumerable
                .Range(0, 100)
                .Select(_ => new QueuedPlayer(Guid.NewGuid(), now.AddMilliseconds(100_000), 42))
                .ForEach(matchMakingQueue.Add);

            var result = matchMakingQueue.Pop(10, matchMaking);
            Assert.AreEqual(10, result.players.Count());
        }

        [Test]
        public async Task OnePlayerMoreThanTime()
        {
            var now = DateTime.UtcNow;
            var matchMakingQueue = new MatchMakingQueue();
            matchMakingQueue.Add(new QueuedPlayer(Guid.NewGuid(), now.AddMilliseconds(-100_000), 42));
            var result = matchMakingQueue.Pop(10, matchMaking);
            Assert.AreEqual(1, result.players.Count());
        }

        [Test]
        public async Task TwoPlayerMoreThanTimeOfSameLevel()
        {
            var now = DateTime.UtcNow;
            var matchMakingQueue = new MatchMakingQueue();
            matchMakingQueue.Add(new QueuedPlayer(Guid.NewGuid(), now.AddMilliseconds(-100_000), 42));
            matchMakingQueue.Add(new QueuedPlayer(Guid.NewGuid(), now.AddMilliseconds(-100_000), 42));
            var result = matchMakingQueue.Pop(10, matchMaking);
            Assert.AreEqual(2, result.players.Count());
        }

        [Test]
        [Ignore("As of now algorithm is simplier, uncomment as soon as more clever")]
        public async Task SkipLevel()
        {
            var matchMakingQueue = new MatchMakingQueue();
            var now = DateTime.UtcNow;
            matchMakingQueue.Add(new QueuedPlayer(Guid.NewGuid(), now.AddMilliseconds(-100_000), 42));
            matchMakingQueue.Add(new QueuedPlayer(Guid.NewGuid(), now.AddMilliseconds(-100_000), 42));
            matchMakingQueue.Add(new QueuedPlayer(Guid.NewGuid(), now.AddMilliseconds(-100_000), 1));
            matchMakingQueue.Add(new QueuedPlayer(Guid.NewGuid(), now.AddMilliseconds(+100_000), 42));
            var result = matchMakingQueue.Pop(10, matchMaking);
            Assert.AreEqual(2, result.players.Count());
            Assert.AreEqual(result.players[0].Level, result.players[1].Level);
        }

        [Test]
        public async Task TwoPlayersMoreThanTimeOfWayToDiffrerentLevel()
        {
            var now = DateTime.UtcNow;
            var matchMakingQueue = new MatchMakingQueue();
            matchMakingQueue.Add(new QueuedPlayer(Guid.NewGuid(), now.AddMilliseconds(-100_000), 1));
            matchMakingQueue.Add(new QueuedPlayer(Guid.NewGuid(), now.AddMilliseconds(-100_000), 666));
            var result = matchMakingQueue.Pop(10, matchMaking);
            Assert.AreEqual(1, result.players.Count());
            var result2 = matchMakingQueue.Pop(10, matchMaking);
            Assert.AreEqual(1, result2.players.Count());
            Assert.False(matchMakingQueue.Any());
        }

        [Test]
        public async Task ManyPlayersAllForGame()
        {
            var now = DateTime.UtcNow;
            var matchMakingQueue = new MatchMakingQueue();
            Enumerable
                .Range(0, 100)
                .Select(_ => new QueuedPlayer(Guid.NewGuid(), now.AddMilliseconds(-100_000), 42))
                .ForEach(matchMakingQueue.Add);

            var result = matchMakingQueue.Pop(10, matchMaking);
            Assert.AreEqual(10, result.players.Count());

            var result2 = matchMakingQueue.Pop(10, matchMaking);
            Assert.AreEqual(10, result2.players.Count());

            var result3 = matchMakingQueue.Pop(10, matchMaking);
            Assert.AreEqual(10, result2.players.Count());
        }
    }
}