using System;
using Heyworks.PocketShooter.SocialConnections.SocialNetworks;

namespace Heyworks.PocketShooter.SocialConnections.Core
{
    /// <summary>
    /// Represents an object managing access to the encapsulated social network.
    /// </summary>
    public sealed class SocialNetworkManager
    {
        private static ISocialNetwork gameCenter;
        private static ISocialNetwork googlePlay;

        /// <summary>
        /// Gets the object managing access to the encapsulated Game Center social network and data received from the network.
        /// </summary>
        public static ISocialNetwork GameCenter =>
            gameCenter ?? (gameCenter = new GameCenterSocialNetwork());

        /// <summary>
        /// Gets the object managing access to the Google Play social network and data received from the network.
        /// </summary>
        public static ISocialNetwork GooglePlay =>
            googlePlay ?? (googlePlay = new GooglePlaySocialNetwork());
    }
}