namespace Heyworks.PocketShooter.Realtime.Connection
{
    public interface IConnection
    {
        /// <summary>
        /// Gets current connection state.
        /// </summary>
        ConnectionState ConnectionState { get; }

        /// <summary>
        /// Initiates the connection to the server.
        /// Changes state to <see cref="Connection.ConnectionState.Connecting"/> in case of success.
        /// </summary>
        void Connect();

        /// <summary>
        ///  Initiates a disconnect between this client and the server.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Gets the server address.
        /// </summary>
        string ServerAddress { get; }
    }
}