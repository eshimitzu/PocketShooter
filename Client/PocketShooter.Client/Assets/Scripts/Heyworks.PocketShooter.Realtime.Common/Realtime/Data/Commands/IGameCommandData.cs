namespace Heyworks.PocketShooter.Realtime.Data
{
    /// <summary>
    /// Implementations of this interfaces are stuctures which are send by client to server as commands.
    /// </summary>
    public interface IGameCommandData
    {
        /// <summary>
        /// Identifiers of some active entity (like trooper or turret).
        /// </summary>
        EntityId ActorId { get; }

        SimulationDataCode Code { get; }
    }
}
