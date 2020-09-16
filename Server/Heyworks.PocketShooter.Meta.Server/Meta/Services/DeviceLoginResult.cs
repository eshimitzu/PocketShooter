namespace Heyworks.PocketShooter.Meta.Services
{
    /// <summary>
    /// Represents user's device login result.
    /// </summary>
    public class DeviceLoginResult : LoginResult
    {
        /// <summary>
        /// Gets or sets the device id.
        /// </summary>
        public string DeviceId
        {
            get;
            set;
        }
    }
}
