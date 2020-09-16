using System.Collections.Immutable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Heyworks.PocketShooter;
using Heyworks.PocketShooter.Meta.Configuration;
using MoreLinq;
using Orleans.Services;
using Heyworks.PocketShooter.Meta.Configuration.Data;

namespace Heyworks.PocketShooter.Meta.Services
{
    // not thread safe
    public class MatchMakingQueue
    {
        private List<QueuedPlayer> dominationQueue = new List<QueuedPlayer>();

        public bool Any() => dominationQueue.Any();

        // ordered by time
        public void Add(QueuedPlayer queuedPlayer)
        {
            if (dominationQueue.Any() && queuedPlayer.RequestedAt < dominationQueue[dominationQueue.Count - 1].RequestedAt)
                Throw.InvalidOperation("Players should be queued by time");
            dominationQueue.Add(queuedPlayer);
        }

        public (int level, IReadOnlyList<QueuedPlayer> players) Pop(int limit, MatchMakingConfiguration matchingConfig)
        {
            // simplistic non optimal, will improve as soon as real scenarios to work out

            // TODO: inject time to allow better testing
            var past = DateTime.UtcNow.AddMilliseconds(-matchingConfig.ForcedStartMs);

            while (true)
            {
                var first = dominationQueue[0];
                var add = Math.Min(limit, dominationQueue.Count);
                var result = new List<QueuedPlayer>(add);
                result.Add(first); add--;
                var i = 1;
                while (add > 0 && i < dominationQueue.Count)
                {
                    var possible = dominationQueue[i];
                    if (Math.Abs(first.Level - possible.Level) <= matchingConfig.InitialLevelSpread)
                    {
                        result.Add(possible);
                        add--;
                    }

                    i++;
                }

                // // took into game only those who waited long if no people presented for full match
                // if (result.Count < modeConfig.MaxPlayers)
                //     result = result.Where(_ => _.RequestedAt <= past).ToList();

                // in reality we must only mark these for deletion and never delete, just overwrite
                // so the queue may only be expanded or throw limit on expansion
                // but never shifted for deletion (slow)
                foreach (var added in result)
                    dominationQueue.Remove(added);

                return (result.Any() ? result.First().Level : 0, result.Shuffle().ToImmutableList());
            }
        }
    }
}
