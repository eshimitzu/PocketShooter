using System;
using Heyworks.PocketShooter.Meta.Data;

namespace Heyworks.PocketShooter.Meta.Entities
{
    /// <summary>
    /// Represents a social network connection.
    /// </summary>
    public abstract class SocialConnection
    {
        private static readonly TimeSpan loginTokenExpirationInterval = TimeSpan.FromDays(1);

        /// <summary>
        /// Initializes a new instance of the <see cref="SocialConnection"/> class.
        /// </summary>
        protected SocialConnection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SocialConnection"/> class and initializes it with the social network user's identifier.
        /// </summary>
        /// <param name="internalId">The social network user's identifier.</param>
        protected SocialConnection(string internalId)
            : this()
        {
            this.InternalId = internalId.NotNull();
        }

        /// <summary>
        /// Gets the social network user's identifier.
        /// </summary>
        public string InternalId
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the date when user logged in last time.
        /// </summary>
        public DateTime LastActivity
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the login token which can be used instead of social network auth token.
        /// </summary>
        public string LoginToken
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the social login token which can be used instead of social network credentials.
        /// </summary>
        public DateTime LoginTokenExpirationDate
        {
            get;
            private set;
        }

        /// <summary>
        /// Sets the date when user logged in last time.
        /// </summary>
        /// <param name="dateTime">The last activity date.</param>
        public void SetLastActivity(DateTime dateTime)
        {
            LastActivity = dateTime;
        }

        /// <summary>
        /// Refreshes the current social login token and its expiration date.
        /// </summary>
        public void RefreshLoginToken()
        {
            this.LoginToken = Guid.NewGuid().ToString("N");
            this.LoginTokenExpirationDate = DateTime.UtcNow.Add(loginTokenExpirationInterval);
        }

        public SocialConnectionState GetState() =>
            new SocialConnectionState
            {
                InternalId = InternalId,
                LoginToken = LoginToken,
                LoginTokenExpirationDate = LoginTokenExpirationDate,
            };
    }
}
