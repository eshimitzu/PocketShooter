namespace Heyworks.PocketShooter.Realtime.Simulation
{
    /// <summary>
    /// Contains states with modifications done for and by local. 
    /// Cannot modify states of remotes.
    /// </summary>
    public interface ILocalStateProvider<T> : IStateProvider<T>
    {
    }
}