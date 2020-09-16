using System;
using System.Text;

namespace System.Collections.Generic
{
    public sealed class CyclicSequence
    {
        private const int EmptySequenceNumber = int.MaxValue;

        private readonly int[] sequence;

        public ReadOnlySpan<int> Sequence => sequence;

        public CyclicSequence(int size)
        {
            if (size <= 0)
            {
                throw new ArgumentException("The size must be greater than 0", nameof(size));
            }

            this.Size = size;
            this.sequence = new int[size];

            Reset();
        }

        /// <summary>
        /// Gets a next number in sequence.
        /// </summary>
        public int NextSequenceNumber
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a sequence size.
        /// </summary>
        public int Size { get; }

        public void Reset()
        {
            NextSequenceNumber = 0;
            for (int i = 0; i < Size; i++)
            {
                sequence[i] = EmptySequenceNumber;
            }
        }

        public int Insert(int sequenceNumber)
        {
            AssertNotEmptySequenceNo(sequenceNumber);
            if (Stale(sequenceNumber)) return -1;

            if (sequenceNumber + 1 > NextSequenceNumber)
            {
                Remove(NextSequenceNumber, sequenceNumber);
                NextSequenceNumber = sequenceNumber + 1;
            }

            int sindex = SIndex(sequenceNumber);
            sequence[sindex] = sequenceNumber;
            return sindex;
        }

        // true if cannot insert sequence because it is out of buffer (too old)
        public bool Stale(int sequenceNumber) => sequenceNumber + Size < NextSequenceNumber;

        public void Remove(int sequenceNumber)
        {
            AssertNotEmptySequenceNo(sequenceNumber);
            sequence[SIndex(sequenceNumber)] = EmptySequenceNumber;
        }

        public void Remove(int startSequenceNumber, int finishSequenceNumber)
        {
            AssertNotEmptySequenceNo(startSequenceNumber);
            AssertNotEmptySequenceNo(finishSequenceNumber);

            if (startSequenceNumber > finishSequenceNumber)
            {
                throw new ArgumentException("The startSequenceNo must be less than or equal to finishSequenceNo", nameof(startSequenceNumber));
            }

            if (finishSequenceNumber - startSequenceNumber < Size)
            {
                for (int sequenceNo = startSequenceNumber; sequenceNo <= finishSequenceNumber; sequenceNo++)
                {
                    Remove(sequenceNo);
                }
            }
            else
            {
                for (int i = 0; i < Size; i++)
                {
                    sequence[i] = EmptySequenceNumber;
                }
            }
        }

        public bool IsAvailable(int sequenceNumber)
        {
            AssertNotEmptySequenceNo(sequenceNumber);
            return Stale(sequenceNumber)
                ? false
                : sequence[SIndex(sequenceNumber)] == EmptySequenceNumber;
        }

        public bool IsExists(int sequenceNumber)
        {
            AssertNotEmptySequenceNo(sequenceNumber);
            return sequence[SIndex(sequenceNumber)] == sequenceNumber;
        }

        public int Find(int sequenceNumber)
        {
            AssertNotEmptySequenceNo(sequenceNumber);
            var sindex = SIndex(sequenceNumber);
            return sequence[sindex] == sequenceNumber ? sindex : -1;
        }

        public int ToIndex(int sequenceNumber) => SIndex(sequenceNumber);

        private static void AssertNotEmptySequenceNo(int sequenceNumber)
        {
            if (sequenceNumber == EmptySequenceNumber)
            {
                throw new ArgumentException($"The sequenceNo must not be equal to {EmptySequenceNumber}", nameof(sequenceNumber));
            }
        }

        private int SIndex(int sequenceNumber) => (sequenceNumber % Size + Size) % Size;
    }
}
