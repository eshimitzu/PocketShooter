using System;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Simulation;
using UniRx;

namespace Heyworks.PocketShooter.Realtime.Entities
{
    public interface IRemotePlayerEvents : IPlayerEvents
    {
        IReadOnlyReactiveProperty<FpsTransformComponent> Moved { get; }

        IObservable<AttackServerEvent> Attack { get; }
    }
}