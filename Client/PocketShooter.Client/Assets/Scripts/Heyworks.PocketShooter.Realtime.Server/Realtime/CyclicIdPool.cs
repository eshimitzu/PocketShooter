using System;
using System.Collections.Generic;
using System.Linq;

namespace Heyworks.PocketShooter.Realtime
{
    public sealed class CyclicIdPool
    {
        private readonly object sync = new object();
        private readonly Queue<EntityId> pool = new Queue<EntityId>(EntityId.MaxValue);
        private readonly HashSet<EntityId> used = new HashSet<EntityId>();

        public CyclicIdPool()
            : this(EntityId.MinValue + 1, EntityId.MaxValue)
        {
        }

        public CyclicIdPool(EntityId minValue, EntityId maxValue)
            : this(Enumerable.Range(minValue, maxValue).Select(value => (EntityId)value))
        {
        }

        public CyclicIdPool(IEnumerable<EntityId> valuesPool)
        {
            foreach (var value in valuesPool)
            {
                if (value != 0)
                {
                    pool.Enqueue(value);
                }
                else
                {
                    throw new ArgumentException("The 0 value is reserved and not allowed.");
                }
            }
        }

        public bool Acquire(out EntityId value)
        {
            lock (sync)
            {
                if (pool.Count > 0)
                {
                    value = pool.Dequeue();
                    used.Add(value);

                    return true;
                }
            }

            value = 0;

            return false;
        }

        public void Release(EntityId value)
        {
            lock (sync)
            {
                if (used.Remove(value))
                {
                    pool.Enqueue(value);
                }
            }
        }
    }
}
