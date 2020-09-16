using System.Collections.Generic;
using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Entities
{

    public static class LocalRef
    {
        public static LocalRef<T> Create<T>(IRefIndex<T> refIndex, Tick tick) 
            where T: struct
            =>
            new LocalRef<T>(refIndex, tick);
    }

    public struct LocalRef<T> : IRef<T> where T:struct
    {
        private IRefIndex<T> refIndex;

        public LocalRef(IRefIndex<T> refIndex, Tick tick)
        {
            this.refIndex = refIndex;
            this.Tick = tick;
        }

        /// <summary>Gets.</summary>
        public ref T Value => ref refIndex[Tick];

        public Tick Tick { get; }
    }
}
