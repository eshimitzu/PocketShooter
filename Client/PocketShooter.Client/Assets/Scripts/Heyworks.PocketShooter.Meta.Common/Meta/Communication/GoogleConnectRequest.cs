namespace Heyworks.PocketShooter.Meta.Communication
{
    /// <summary>
    /// Contains data coming with Google social connect request.
    /// </summary>
    public class GoogleConnectRequest : SocialConnectRequest
    {
        /// <summary>
        /// Gets or sets the client access token.
        /// </summary>
        public string ClientAccessToken { get; set; }
    }
}
