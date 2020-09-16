namespace Heyworks.PocketShooter.Meta.Services
{
    /// <summary>
    /// Represents a container with properties required for social login.
    /// </summary>
    public class SocialLoginProperties : LoginProperties
    {
        /// <summary>
        /// Gets or sets the user id in social network.
        /// </summary>
        public string SocialId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the login token which can be used instead of social network credentials.
        /// </summary>
        public string LoginToken
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether the social login token property is set.
        /// </summary>
        public bool HasLoginToken => !string.IsNullOrEmpty(LoginToken);
    }
}
