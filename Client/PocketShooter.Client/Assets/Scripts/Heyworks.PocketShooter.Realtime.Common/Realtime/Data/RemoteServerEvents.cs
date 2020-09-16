namespace Heyworks.PocketShooter.Realtime.Data
{
    /// <summary>
    /// Events send by server about each player to all.
    /// </summary>
    public struct RemoteServerEvents
    {
        /// <summary>
        /// Who killed this player.
        /// </summary>
        public EntityId LastKiller;
    }
}