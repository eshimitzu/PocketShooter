namespace Heyworks.PocketShooter.Meta.Services
{
    /// <summary>
    /// Represents a container with properties required for Google connect.
    /// </summary>
    public class GoogleConnectProperties : SocialConnectProperties
    {
        /// <summary>
        /// Gets or sets the Google's client access token.
        /// </summary>
        public string AccessToken
        {
            get;
            set;
        }
    }
}
