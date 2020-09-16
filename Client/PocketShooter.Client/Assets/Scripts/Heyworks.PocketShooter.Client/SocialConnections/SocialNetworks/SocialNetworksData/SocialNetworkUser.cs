namespace Heyworks.PocketShooter.SocialConnections.SocialNetworks
{
    /// <summary>
    /// Represents the object containing information about user of some social network.
    /// </summary>
    public class SocialNetworkUser
    {
        /// <summary>
        /// Gets the user's ID in social network.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets or sets the user's first name in social network.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the user's avatar URL in social network.
        /// </summary>
        public string AvatarURL { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SocialNetworkUser"/> class.
        /// </summary>
        /// <param name="id"> User's ID in social network. </param>
        public SocialNetworkUser(string id) => Id = id;

        /// <summary>
        /// Initializes a new instance of the <see cref="SocialNetworkUser"/> class.
        /// </summary>
        /// <param name="id"> User's ID in social network. </param>
        /// <param name="firstName"> User's first name in social network. </param>
        /// <param name="avatarUrl"> User's avatar URL in social network. </param>
        public SocialNetworkUser(string id, string firstName, string avatarUrl)
        {
            Id = id;
            FirstName = firstName;
            AvatarURL = avatarUrl;
        }
    }
}
