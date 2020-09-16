using System;

namespace Heyworks.PocketShooter.Realtime
{
    public sealed class CyclicSequence
    {
        private const int EmptySequenceNo = int.MaxValue;

        private readonly int[] sequence;

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
        public int NextSequenceNo
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a sequence size.
        /// </summary>
        public int Size
        {
            get;
        }

        public void Reset()
        {
            NextSequenceNo = 0;
            for (int i = 0; i < Size; i++)
            {
                sequence[i] = EmptySequenceNo;
            }
        }

        public int Insert(int sequenceNo)
        {
            AssertNotEmptySequenceNo(sequenceNo);

            if (sequenceNo + 1 > NextSequenceNo)
            {
                Remove(NextSequenceNo, sequenceNo);

                NextSequenceNo = sequenceNo + 1;
            }
            else if (sequenceNo < NextSequenceNo - Size)
            {
                return -1;
            }

            int index = Mod(sequenceNo, Size);

            sequence[index] = sequenceNo;

            return index;
        }

        public void Remove(int sequenceNo)
        {
            AssertNotEmptySequenceNo(sequenceNo);

            sequence[Mod(sequenceNo, Size)] = EmptySequenceNo;
        }

        public void Remove(int startSequenceNo, int finishSequenceNo)
        {
            AssertNotEmptySequenceNo(startSequenceNo);
            AssertNotEmptySequenceNo(finishSequenceNo);

            if (startSequenceNo > finishSequenceNo)
            {
                throw new ArgumentException("The startSequenceNo must be less than or equal to finishSequenceNo", nameof(startSequenceNo));
            }

            if (finishSequenceNo - startSequenceNo < Size)
            {
                for (int sequenceNo = startSequenceNo; sequenceNo <= finishSequenceNo; sequenceNo++)
                {
                    Remove(sequenceNo);
                }
            }
            else
            {
                for (int i = 0; i < Size; i++)
                {
                    sequence[i] = EmptySequenceNo;
                }
            }
        }

        public bool IsAvailable(int sequenceNo)
        {
            AssertNotEmptySequenceNo(sequenceNo);

            if (sequenceNo < NextSequenceNo - Size)
            {
                return false;
            }

            return sequence[Mod(sequenceNo, Size)] == EmptySequenceNo;
        }

        public bool IsExists(int sequenceNo)
        {
            AssertNotEmptySequenceNo(sequenceNo);

            return sequence[Mod(sequenceNo, Size)] == sequenceNo;
        }

        public int Find(int sequenceNo)
        {
            AssertNotEmptySequenceNo(sequenceNo);

            int index = Mod(sequenceNo, Size);

            if (sequence[index] == sequenceNo)
            {
                return index;
            }
            else
            {
                return -1;
            }
        }

        private static void AssertNotEmptySequenceNo(int sequenceNo)
        {
            if (sequenceNo == EmptySequenceNo)
            {
                throw new ArgumentException($"The sequenceNo must not be equal to {EmptySequenceNo}", nameof(sequenceNo));
            }
        }

        private static int Mod(int x, int m)
        {
            return (x % m + m) % m;
        }
    }
}
