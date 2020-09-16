using System;
using System.Net;

namespace Heyworks.PocketShooter.Meta.Communication
{
    /// <summary>
    /// Stores information about entry of realtime server registration.
    /// </summary>
    public class RealtimeServerMetaEntry {
        /// <summary>
        /// Meta events stream subscriber.
        /// </summary>
        public RealtimeServerMetaEntry (IMatchMakedObserver observer, DateTime utcSince, IPEndPoint address) {
            this.Observer = observer;
            this.UtcSince = utcSince;
            this.Address = address;
        }

        private RealtimeServerMetaEntry(){}

        public IMatchMakedObserver Observer { get; }

        /// <summary>
        /// The time of registration.
        /// </summary>
        public DateTime UtcSince { get; }

        /// <summary>
        /// Where clients should go to play.
        /// </summary>
        public IPEndPoint Address { get; }
    }
}