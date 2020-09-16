using System.Text;

namespace System.Collections.Generic
{
    public class SafeCyclicSequenceBuffer<T> : IRefList<T>
    {
        private readonly SafeCyclicSequence cyclicSequence;
        private readonly T[] items;

        public SafeCyclicSequenceBuffer(int size)
        {
            this.cyclicSequence = new SafeCyclicSequence(size);
            this.items = new T[size];
        }

        /// <inheritdoc/>
        public bool TryInsert(int sequenceNumber, in T item)
        {
            var index = cyclicSequence.Insert(sequenceNumber);
            if (index >= 0)
            {
                items[index] = item;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Conains sequenc number.
        /// </summary>
        public bool ContainsKey(int sequenceNumber) => cyclicSequence.ContainsKey(sequenceNumber);

        public bool CanInsert(int sequenceNumber) => !IsStale(sequenceNumber) && !ContainsKey(sequenceNumber);

        /// <inheritdoc/>
        public void Insert(int sequenceNumber, in T item)
        {
            var index = cyclicSequence.Insert(sequenceNumber);
            if (index < 0)
            {
                Throw.InvalidOperation($"Failed to insert {sequenceNumber} with value {item}");
            }

            items[index] = item;
        }

        public bool TryGetValue(int sequenceNumber, out T value)
        {
            int index = cyclicSequence.Find(sequenceNumber);
            if (index >= 0)
            {
                value = items[index];
                return true;
            }

            value = default;
            return false;
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
        public ref T this[int sequenceNumber]
        {
            get
            {
                int index = cyclicSequence.Find(sequenceNumber);
                if (index < 0)
                {
                    Throw.Argument($"No item by index {sequenceNumber}");
                }

                return ref items[index];
            }
        }

        public bool IsStale(int sequenceNumber) => cyclicSequence.IsStale(sequenceNumber);
    }
}