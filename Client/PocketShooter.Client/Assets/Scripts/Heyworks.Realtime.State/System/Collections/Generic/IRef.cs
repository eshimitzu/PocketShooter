namespace System.Collections.Generic
{
    /// <summary>
    /// Strongly typed reference to struct. Can be copied any times. Size should be small. E.g. equal or less to IntPtr.Size.
    /// Cannot `use` normal C# share as it cannot be stored into object by ref.
    /// Do not box it, but use constrained generics for performane.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    // try to find such or similar interface in .net framework and use it
    public interface IRef<T>
        where T : struct
    {
        /// <summary>
        /// Gets value by reference.
        /// </summary>
        /// <value>The value.</value>
        ref T Value { get; }
    }
}
