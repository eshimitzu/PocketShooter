namespace Heyworks.PocketShooter.Meta.Communication
{
    /// <summary>
    /// Contains fields coming with the login request.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Gets or sets the device unique id.
        /// </summary>
        public string DeviceId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the player's application bundle identifier.
        /// </summary>
        public string BundleId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the player's application store name.
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
