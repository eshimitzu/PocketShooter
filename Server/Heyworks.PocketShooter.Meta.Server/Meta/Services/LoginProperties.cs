namespace Heyworks.PocketShooter.Meta.Services
{
    /// <summary>
    /// Represented a container with properties required for user login.
    /// </summary>
    public class LoginProperties
    {
        /// <summary>
        /// Gets or sets the device unique ID.
        /// </summary>
        public string DeviceId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the user's application bundle identifier.
        /// </summary>
        public string BundleId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the user's application store name.
        /// </summary>
        public ApplicationStoreName ApplicationStore
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the game client version.
        /// </summary>
        public string ClientVersion
        {
            get;
            set;
        }
    }
}
