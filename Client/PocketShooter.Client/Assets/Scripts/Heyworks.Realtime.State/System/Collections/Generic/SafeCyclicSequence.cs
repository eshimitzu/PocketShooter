using System;
using System.Text;

namespace System.Collections.Generic
{
    public sealed class SafeCyclicSequence
    {
        private const int EmptySequenceNumber = int.MaxValue;

        private readonly int[] sequence;

        public ReadOnlySpan<int> Sequence => sequence;

        public SafeCyclicSequence(int size)
        {
            if (size <= 0)
            {
                Throw.Argument("The size must be greater than 0", nameof(size));
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
        
        public int LastSequenceNumber => NextSequenceNumber - 1;

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
            if (IsStale(sequenceNumber)) return -1;

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
        public bool IsStale(int sequenceNumber) => sequenceNumber + Size < NextSequenceNumber;

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
                Throw.Argument("The startSequenceNo must be less than or equal to finishSequenceNo", nameof(startSequenceNumber));
            }
            else
            {
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

        }

        public bool ContainsKey(int sequenceNumber)
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
                Throw.Argument($"The sequenceNo must not be equal to {EmptySequenceNumber}", nameof(sequenceNumber));
            }
        }

        private int SIndex(int sequenceNumber) => (sequenceNumber % Size + Size) % Size;
    }
}
