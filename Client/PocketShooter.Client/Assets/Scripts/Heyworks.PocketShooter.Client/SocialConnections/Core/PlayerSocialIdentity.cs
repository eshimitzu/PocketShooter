using System;

namespace Heyworks.PocketShooter.SocialConnections.Core
{
    /// <summary>
    /// Represents an object, encapsulating all IDs of player for all possible social networks.
    /// </summary>
    public sealed class PlayerSocialIdentity
    {
        /// <summary>
        /// Gets player's ID in the GameCenter social network.
        /// </summary>
        public string GameCenterId { get; private set; }

        /// <summary>
        /// Gets player's ID in the Google Play social network.
        /// </summary>
        public string GooglePlayId { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerSocialIdentity"/> class.
        /// </summary>
        /// <param name="gameCenterId"> Player's ID in the GameCenter social network. </param>
        /// <param name="googlePlayId"> Player's ID in the Google Play social network. </param>
        public PlayerSocialIdentity(string gameCenterId, string googlePlayId)
        {
            GameCenterId = gameCenterId;
            GooglePlayId = googlePlayId;
        }

        /// <summary>
        /// Compares two <see cref="PlayerSocialIdentity"/> objects.
        /// </summary>
        /// <param name="lhsId">The first <see cref="PlayerSocialIdentity"/> object.</param>
        /// <param name="rhsId">The second <see cref="PlayerSocialIdentity"/> object.</param>
        /// <returns>True if the two <see cref="PlayerSocialIdentity"/> objects are equal; otherwise - false.</returns>
        public static bool operator ==(PlayerSocialIdentity lhsId, PlayerSocialIdentity rhsId) => Equals(lhsId, rhsId);

        /// <summary>
        /// Compares two <see cref="PlayerSocialIdentity"/> objects.
        /// </summary>
        /// <param name="lhsId">The first <see cref="PlayerSocialIdentity"/> object.</param>
        /// <param name="rhsId">The second <see cref="PlayerSocialIdentity"/> object.</param>
        /// <returns>True if the two <see cref="PlayerSocialIdentity"/> objects are not equal; otherwise - false.</returns>
        public static bool operator !=(PlayerSocialIdentity lhsId, PlayerSocialIdentity rhsId) => !Equals(lhsId, rhsId);

        /// <summary>
        /// Gets a value indicating whether some player's social ID equals to the specified one.
        /// </summary>
        /// <param name="id"> Id to check for equality with some of player's social IDs. </param>
        public bool HasId(string id) => !string.IsNullOrEmpty(id) && (id == GameCenterId || id == GooglePlayId);

        /// <summary>
        /// Gets the player's ID in the social network with specified name.
        /// </summary>
        /// <param name="socialNetworkName"> Name of the social network. </param>
        public string GetUserSocialNetworkId(SocialNetworkName socialNetworkName)
        {
            switch (socialNetworkName)
            {
                case SocialNetworkName.GameCenter:
                    return GameCenterId;

                case SocialNetworkName.GooglePlay:
                    return GooglePlayId;
                default:
                    throw new ArgumentException($"There is no social network with name {socialNetworkName}");
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="other">T he <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.
        /// </param><filterpriority>2.</filterpriority>
        public override bool Equals(object other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return other is PlayerSocialIdentity identity && Equals(identity);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = GameCenterId != null ? GameCenterId.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (GooglePlayId != null ? GooglePlayId.GetHashCode() : 0);
                return hashCode;
            }
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"GameCenterId = {GameCenterId} GooglePlayId = {GooglePlayId}";
        }

        /// <summary>
        /// Gets a value indicating whether two player social IDs are equal.
        /// </summary>
        /// <param name="id1"> The first player's social ID. </param>
        /// <param name="id2"> The second player's social ID. </param>
        private static bool Equals(PlayerSocialIdentity id1, PlayerSocialIdentity id2) =>
            id1?.Equals(id2) ?? ReferenceEquals(id2, null);

        private bool Equals(PlayerSocialIdentity other) => string.Equals(GameCenterId, other.GameCenterId)
                                                           && string.Equals(GooglePlayId, other.GooglePlayId);
    }
}