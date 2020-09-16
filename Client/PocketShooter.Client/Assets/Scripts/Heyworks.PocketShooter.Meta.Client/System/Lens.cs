using System;

// valid namespace
namespace System
{
    /// <summary>
    /// Read and write immutable data (simplify access and modification for deep part of immutable value). Should be used via LensExtensions.
    /// </summary>
    /// <seealso href="https://github.com/dotnet/csharplang/issues/302"/>
    /// <seealso href="https://github.com/dadhi/ImTools/issues/4"/>    
    /// <remarks>
    /// Runtime overhead approach with delegate. 
    /// If you need zero runtime overhead probably should try do not use immutable at all 
    /// or implement OOP/Interface lenses with large code overhead.
    /// </remarks>
    /// <seealso href="https://medium.com/@gcanti/introduction-to-optics-lenses-and-prisms-3230e73bfcfe"/>
    /// <seealso href="https://github.com/AArnott/ImmutableObjectGraph"/>
    public struct Lens<TWhole, TPart>
    {
        // TODO: create Lens based on Expression<Func> instead of Func to allow automatic parallelism/order of calls for data/merge and dependency check before/after doing calls for data.

        /// <summary>
        /// Gets value of whole from part.
        /// </summary>
        public readonly Func<TWhole, TPart> Get;

        /// <summary>
        /// Sets value of part and creates new whole with this.
        /// </summary>
        /// <remarks>
        /// Goes whole -> part -> whole, not part -> whole -> whole as we do not have partial application here.
        /// </remarks>
        public readonly Func<TWhole, TPart, TWhole> Set;

        /// <summary>
        /// Creates new object to read and write immutable object graphs.
        /// </summary>
        /// <param name="getter">Setter.</param>
        /// <param name="setter">Getter.</param>
        public Lens(Func<TWhole, TPart> getter, Func<TWhole, TPart, TWhole> setter)
        {
            Get = getter ?? throw new ArgumentNullException(nameof(getter));
            Set = setter ?? throw new ArgumentNullException(nameof(setter));
        }

        /// <summary>
        /// Creates new lens.
        /// </summary>
        /// <param name="getter">Gets value of part from whole.</param>
        /// <param name="setter">Sets value of part in whole and creates new whole.</param>
        /// <returns>Object to read and write immutable object.</returns>
        public static Lens<TWhole, TPart> Create(
            Func<TWhole, TPart> getter, Func<TWhole, TPart, TWhole> setter) =>
            new Lens<TWhole, TPart>(getter, setter);
    }

    /// <summary>
    /// Read and write immutable data (simplify access and modification for deep part of immutable value).
    /// </summary>
    public static class LensExtensions
    {
        /// <summary>
        /// Creates new lens on any object. 
        /// Suitable when object graph first createn and than all lenses are created and attached to some holder.
        /// Reduces boiler plate.
        /// </summary>
        /// <typeparam name="TWhole">Type of whole.</typeparam>
        /// <typeparam name="TPart">Type of part inside whole.</typeparam>
        /// <param name="_">Used only for type inference. Pass null or call on object. Ignored.</param>
        /// <param name="getter">Gets value of part from whole.</param>
        /// <param name="setter">Sets value of part in whole and creates new whole.</param>
        /// <returns>Object to read and write immutable object.</returns>
        public static Lens<TWhole, TPart> Lens<TWhole, TPart>(
            this TWhole _,
            Func<TWhole, TPart> getter, Func<TWhole, TPart, TWhole> setter) =>
            new Lens<TWhole, TPart>(getter, setter);

        /// <summary>
        /// Updates value in part.
        /// </summary>
        /// <typeparam name="TWhole">Type of whole.</typeparam>
        /// <typeparam name="TPart">Type of part.</typeparam>
        /// <param name="lens">Lens to use for update.</param>
        /// <param name="whole">Whole object.</param>
        /// <param name="update">Function to update part (create new updated).</param>
        /// <returns>Whole with updated value inside part.</returns>
        public static TWhole Update<TWhole, TPart>(
            this Lens<TWhole, TPart> lens,
            TWhole whole, Func<TPart, TPart> update)
        {
            if (update == null) throw new ArgumentNullException(nameof(update));
            return lens.Set(whole, update(lens.Get(whole)));
        }
        /// <summary>
        /// Allows to access deep hierarchy from parent to mid to child.
        /// </summary>
        /// <typeparam name="TWhole">Root parent. Whole.</typeparam>
        /// <typeparam name="TPart">Intermediate to access from parent to child. Part.</typeparam>
        /// <typeparam name="TSubpart">Deepest child. Sub part.</typeparam>
        /// <param name="parent">From parent lens.</param>
        /// <param name="child">To child lens.</param>
        /// <returns>Lens over three objects.</returns>
        public static Lens<TWhole, TSubpart> Compose<TWhole, TPart, TSubpart>(
            this Lens<TWhole, TPart> parent, Lens<TPart, TSubpart> child) =>
            new Lens<TWhole, TSubpart>(
              whole => child.Get(parent.Get(whole)),
              (whole, part) => parent.Set(whole, child.Set(parent.Get(whole), part)));

        /// <summary>
        /// More C#ish way to get whole.part using lens: `foo.Get(BarLens);`
        /// The bonus is automatically popping down intellisense with
        /// all available lens.
        /// </summary>
        /// <typeparam name="TWhole">e.g. a property holder type</typeparam>
        /// <typeparam name="TPart">e.g. a property type</typeparam>
        /// <param name="whole">e.g. a property holder</param>
        /// <param name="lens">e.g. a getter and setter to access property</param>
        /// <returns>e.g. property value.</returns>
        public static TPart Get<TWhole, TPart>(this TWhole whole, Lens<TWhole, TPart> lens) => lens.Get(whole);

        /// <summary>
        /// More C#ish way to set whole.part using lens: `foo.Set(BarLens, new Bar());`
        /// The bonus is automatically popping down intellisense with
        /// all available lens.
        /// </summary>
        /// <typeparam name="TWhole">e.g. a property holder type</typeparam>
        /// <typeparam name="TPart">e.g. a property type</typeparam>
        /// <param name="whole">e.g. a property holder</param>
        /// <param name="lens">e.g. a getter and setter to access property</param>
        /// <param name="part">e.g. new property value </param>
        /// <returns>e.g. new <typeparamref name="TWhole"/> with set <typeparamref name="TPart"/></returns>
        public static TWhole Set<TWhole, TPart>(this TWhole whole, Lens<TWhole, TPart> lens, TPart part) => 
            lens.Set(whole, part);
    }
}
