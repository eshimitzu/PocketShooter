using Heyworks.PocketShooter.Realtime.Channels;

namespace Heyworks.PocketShooter.Realtime
{
    /// <summary>
    /// Defines an interface for the factory that responsible for creating client states.
    /// </summary>
    public interface IClientStateFactory
    {
        /// <summary>
        /// Creates client state.
        /// </summary>
        /// <param name="message">The message.</param>
        IClientState CreateState(IMessage message);
    }
}
