using System;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Simulation;
using UniRx;

namespace Heyworks.PocketShooter.Realtime.Entities
{
    public class RemotePlayerEvents : PlayerEventsBase, IRemotePlayerEvents
    {
        internal Subject<AttackServerEvent> Attacks = new Subject<AttackServerEvent>();

        public IObservable<AttackServerEvent> Attack => Attacks;

        internal new readonly Subject<FpsTransformComponent> Move;

        internal RemotePlayerEvents(EntityId id)
            : base(id)
        {
            Move = new Subject<FpsTransformComponent>();
            Moved = new ReadOnlyReactiveProperty<FpsTransformComponent>(Move);
        }

        public IReadOnlyReactiveProperty<FpsTransformComponent> Moved { get; }
    }
}