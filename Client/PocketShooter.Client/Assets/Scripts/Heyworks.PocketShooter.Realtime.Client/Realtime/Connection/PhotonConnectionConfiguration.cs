namespace Heyworks.PocketShooter.Realtime.Connection
{
    /// <summary>
    /// Data for connecting to Photon server.
    /// </summary>
    public class PhotonConnectionConfiguration
    {
        private const int PhotonOverhead = 20; //https://doc.photonengine.com/en-us/realtime/current/reference/binary-protoco

        public const int GuarantedMTU = NetStack.Serialization.BitBufferLimits.MtuIeee802 - PhotonOverhead;

        /// <summary>
        /// The name of photon connection.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets address of the Photon server. Format: ip:port (e.g. 127.0.0.1:5055) or hostname:port (e.g. localhost:5055)
        /// </summary>
        public string ServerAddress { get; set; }

        /// <summary>
        /// The name of the application to use within Photon or the appId of PhotonCloud.
        /// Should match a "Name" for an application, as setup in your config.
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// If true, application call EstablishEncryption method on connect.
        /// </summary>
        public bool EstablishEncryption { get; set; }
    }
}