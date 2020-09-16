namespace Heyworks.PocketShooter.Meta.Entities
{
    /// <summary>
    /// Represents a game center social network connection.
    /// </summary>
    public sealed class GameCenterConnection : SocialConnection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameCenterConnection"/> class and initializes it with the social network user's identifier.
        /// </summary>
        /// <param name="internalId">The social network user's identifier.</param>
        public GameCenterConnection(string internalId)
            : base(internalId)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameCenterConnection"/> class.
        /// </summary>
        private GameCenterConnection()
        {
        }
    }
}
