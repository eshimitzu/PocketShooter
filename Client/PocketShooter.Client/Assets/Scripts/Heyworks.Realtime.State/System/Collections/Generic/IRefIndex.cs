namespace System.Collections.Generic
{
    /// <summary>
    /// Allows to return item by index into reference.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    // note: may try to find such interface in .net framework and use
    public interface IRefIndex<T>
    {
        /// <summary>
        /// Gets reference onto value.
        /// </summary>
        /// <param name="index">The index value.</param>
        /// <value>Positive value of index.</value>
        ref T this[int index] { get; }
    }
}