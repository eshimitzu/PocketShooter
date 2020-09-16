using System;
using UniRx;

namespace Heyworks.PocketShooter.Realtime.Entities
{
    public class ClientPlayerEvents : PlayerEventsBase, IClientPlayerEvents
    {
        /// <summary>
        /// The ammo changed.
        /// </summary>
        internal readonly Subject<int> ChangeAmmo = new Subject<int>();

        /// <summary>
        /// Gets the ammo changed.
        /// </summary>
        /// <value>
        /// The ammo changed.
        /// </value>
        public IObservable<int> AmmoChanged => ChangeAmmo;

        internal ClientPlayerEvents(EntityId id)
            : base(id)
        {
        }
    }
}