namespace Heyworks.PocketShooter.Meta.Entities
{
    /// <summary>
    /// Represents a Google game services connection.
    /// </summary>
    public sealed class GoogleConnection : SocialConnection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleConnection"/> class and initializes it with the social network user's identifier.
        /// </summary>
        /// <param name="internalId">The social network user's identifier.</param>
        public GoogleConnection(string internalId)
            : base(internalId)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleConnection"/> class.
        /// </summary>
        private GoogleConnection()
        {
        }
    }
}
