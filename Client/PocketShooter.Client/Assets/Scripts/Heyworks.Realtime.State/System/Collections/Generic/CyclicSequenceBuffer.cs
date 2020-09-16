using Microsoft.Extensions.Logging;

namespace System.Collections.Generic
{
    public class CyclicSequenceBuffer<T> : IRefList<T>
    {
        private readonly CyclicSequence cyclicSequence;
        private readonly T[] items;
        private readonly string tag;

        public CyclicSequenceBuffer(int size, string tag)
            :this(size)
        {
            this.tag = tag;
        }

        public CyclicSequenceBuffer(int size, Func<T> itemFactory)
            : this(size)
        {
            for (int i = 0; i < size; i++)
            {
                items[i] = itemFactory();
            }
        }

        public CyclicSequenceBuffer(int size)
        {
            this.cyclicSequence = new CyclicSequence(size);
            this.items = new T[size];
        }

        public int Size => items.Length;

        /// <inheritdoc/>
        public bool TryInsert(int sequenceNo, in T item)
        {
            var index = cyclicSequence.Insert(sequenceNo);
            if (index >= 0)
            {
                items[index] = item;

                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public bool ContainsKey(int sequenceNo) => cyclicSequence.IsExists(sequenceNo);

        /// <inheritdoc/>
        public void Insert(int sequenceNo, in T item)
        {
            var index = cyclicSequence.Insert(sequenceNo);
            if (index >= 0)
            {
                items[index] = item;
            }
            else
            {
                throw new InvalidOperationException("Failed to insert");
            }
        }
        
        /// <summary>
        /// Try to get item currently placed by sequenceNo.
        /// </summary>
        /// <param name="sequenceNo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool TryGetItemInPlace(int sequenceNo, out T item)
        {
            var index = cyclicSequence.ToIndex(sequenceNo);
            if (index >= 0)
            {
                item = items[index];
                return true;
            }

            item = default(T);
            return false;
        }

        /// <inheritdoc/>
        public ref T this[int sequenceNo]
        {
            get
            {
                int index = cyclicSequence.Find(sequenceNo);
                if (index >= 0)
                {
                    return ref items[index];
                }

                // If no errors appears delete this class and use SafeCyclicSequenceBuffer
                SimulationLog.Log.LogError(
                    $"[DUMMY STATE] No item by index {sequenceNo}. Last index is {cyclicSequence.NextSequenceNumber - 1}, size is {Size}.");
                throw new ArgumentException(
                    $"[DUMMY STATE] No item by index {sequenceNo}. Last index is {cyclicSequence.NextSequenceNumber - 1}, size is {Size}.");
            }
        }
    }
}