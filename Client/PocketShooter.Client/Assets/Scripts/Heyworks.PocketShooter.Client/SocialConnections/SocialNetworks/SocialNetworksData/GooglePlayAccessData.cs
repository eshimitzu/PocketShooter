namespace Heyworks.PocketShooter.SocialConnections.SocialNetworks
{
    /// <summary>
    /// Represents an object containing information necessary to access any oauth social network.
    /// </summary>
    public class GooglePlayAccessData : SocialNetworkAccessData
    {
        /// <summary>
        /// Gets the social network's access token.
        /// </summary>
        public string AccessToken { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GooglePlayAccessData"/> class.
        /// </summary>
        /// <param name="accessToken"> Social network's access token. </param>
        /// <param name="userId"> User ID in the social network. </param>
        public GooglePlayAccessData(string accessToken, string userId)
            : base(userId)
        {
            AccessToken = accessToken;
        }
    }
}