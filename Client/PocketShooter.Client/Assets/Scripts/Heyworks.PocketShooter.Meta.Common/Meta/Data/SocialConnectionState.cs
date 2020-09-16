using System;

namespace Heyworks.PocketShooter.Meta.Data
{
    /// <summary>
    /// Defines a state of player's social connection.
    /// </summary>
    public class SocialConnectionState
    {
        /// <summary>
        /// Gets or sets the user id in social network.
        /// </summary>
        public string InternalId
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
        /// Gets or sets the expiration date of the login token.
        /// </summary>
        public DateTime LoginTokenExpirationDate
        {
            get;
            set;
        }
    }
}
