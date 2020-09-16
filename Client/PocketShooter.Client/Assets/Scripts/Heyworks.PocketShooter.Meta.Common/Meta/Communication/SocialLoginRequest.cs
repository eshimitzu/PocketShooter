namespace Heyworks.PocketShooter.Meta.Communication
{
    /// <summary>
    ///  Contains fields coming with the social login request.
    /// </summary>
    public class SocialLoginRequest : LoginRequest
    {
        /// <summary>
        /// Gets or sets the social internal user id.
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
    }
}
