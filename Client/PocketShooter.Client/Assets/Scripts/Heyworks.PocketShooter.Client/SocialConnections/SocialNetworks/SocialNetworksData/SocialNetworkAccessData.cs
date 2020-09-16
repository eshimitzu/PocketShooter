namespace Heyworks.PocketShooter.SocialConnections.SocialNetworks
{
    /// <summary>
    /// Represents an object containing information necessary to access any social network.
    /// </summary>
    public abstract class SocialNetworkAccessData
    {
        /// <summary>
        /// Gets the user ID in the social network.
        /// </summary>
        public string UserId { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SocialNetworkAccessData"/> class.
        /// </summary>
        /// <param name="userId"> User ID in the social network. </param>
        protected SocialNetworkAccessData(string userId)
        {
            UserId = userId;
        }
    }
}
