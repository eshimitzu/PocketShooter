using System;

namespace Heyworks.PocketShooter.Realtime.Entities
{
    public interface IClientPlayerEvents : IPlayerEvents
    {
        IObservable<int> AmmoChanged { get; }
    }
}