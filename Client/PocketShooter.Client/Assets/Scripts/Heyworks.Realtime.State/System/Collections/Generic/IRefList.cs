namespace System.Collections.Generic
{
    public interface IRefList<T> : IRefIndex<T>
    {
        /// <summary>
        /// Inserts item into index.
        /// </summary>
        void Insert(int index, in T item);

        /// <summary>
        /// Inserts item into index.
        /// </summary>
        bool TryInsert(int index, in T item);

        /// <summary>
        /// Determines if item with this number exists in the collection.
        /// </summary>
        /// <param name="sequenceNo">Number of the item.</param>
        /// <returns>True if exists.</returns>
        bool ContainsKey(int sequenceNo);

    }
}