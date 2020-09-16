using System;

namespace Heyworks.PocketShooter.Realtime.Simulation
{
    /// <summary>
    /// Events which are raised on behalf on any acting entity (trooper or turret)
    /// </summary>
    public interface IActorEvent
    {
        EntityId ActorId {get;}
    }
}