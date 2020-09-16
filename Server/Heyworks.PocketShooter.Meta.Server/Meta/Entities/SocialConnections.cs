using System;
using System.Collections.Generic;

namespace Heyworks.PocketShooter.Meta.Entities
{
    /// <summary>
    /// Represents entity containing information about player social connections.
    /// </summary>
    public class SocialConnections
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SocialConnections"/> class.
        /// </summary>
        internal SocialConnections()
        {
            ResetAll();
        }

        /// <summary>
        /// Gets a GameCenter connection settings.
        /// </summary>
        public SocialConnection GameCenter
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the Google game services connection settings.
        /// </summary>
        public SocialConnection Google
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the social connection by connection type.
        /// </summary>
        /// <param name="connectionType">The type of social connection.</param>
        /// <returns>A specific Social Connection instance.</returns>
        public SocialConnection GetConnection(Type connectionType)
        {
            if (connectionType == typeof(GoogleConnection))
            {
                return Google;
            }
            else if (connectionType == typeof(GameCenterConnection))
            {
                return GameCenter;
            }
            else
            {
                throw new NotImplementedException($"The social connection of type {connectionType.Name} is not supported");
            }
        }

        /// <summary>
        /// Gets the boolean value indicating whether the social connection of given type exists.
        /// </summary>
        /// <param name="connectionType">The type of social connection.</param>
        public bool HasConnection(Type connectionType)
        {
            return GetConnection(connectionType) != null;
        }

        /// <summary>
        /// Connects the given social account to player profile.
        /// </summary>
        /// <param name="socialConnection">A social connection instance to connect.</param>
        public void Connect(SocialConnection socialConnection)
        {
            socialConnection.NotNull();

            if (HasConnection(socialConnection.GetType()))
            {
                throw new InvalidOperationException("Could not create a new social connection because another social connection of the same type already exists.");
            }

            SetSocialConnection(socialConnection.GetType(), socialConnection);
        }

        /// <summary>
        /// Resets a social connection of the given type.
        /// </summary>
        /// <param name="socialConnectionType">The social connection type to reset.</param>
        public SocialConnection Reset(Type socialConnectionType)
        {
            var connectionToReset = GetConnection(socialConnectionType);

            SetSocialConnection(socialConnectionType, null);

            return connectionToReset;
        }

        /// <summary>
        /// Clears all available social connections.
        /// </summary>
        public void ResetAll()
        {
            foreach (var connectionType in ConnectionTypes)
            {
                if (HasConnection(connectionType))
                {
                    Reset(connectionType);
                }
            }
        }

        #region [Private members]

        private static IEnumerable<Type> ConnectionTypes
        {
            get
            {
                return new List<Type>
                {
                    typeof(GoogleConnection), typeof(GameCenterConnection),
                };
            }
        }

        private void SetSocialConnection(Type connectionType, SocialConnection newSocialConnection)
        {
            if (connectionType == typeof(GoogleConnection))
            {
                Google = newSocialConnection;
            }
            else if (connectionType == typeof(GameCenterConnection))
            {
                GameCenter = newSocialConnection;
            }
            else
            {
                throw new NotImplementedException($"The social connection of type {connectionType.Name} is not supported");
            }
        }

        #endregion [Private members]
    }
}
