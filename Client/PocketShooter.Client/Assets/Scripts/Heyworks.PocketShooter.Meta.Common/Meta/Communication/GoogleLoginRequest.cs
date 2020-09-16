namespace Heyworks.PocketShooter.Meta.Communication
{
    /// <summary>
    /// Contains fields coming with the Google social login request.
    /// </summary>
    public class GoogleLoginRequest : SocialLoginRequest
    {
        /// <summary>
        /// Gets or sets the client access token.
        /// </summary>
        public string ClientAccessToken
        {
            get;
            set;
        }
    }
}
