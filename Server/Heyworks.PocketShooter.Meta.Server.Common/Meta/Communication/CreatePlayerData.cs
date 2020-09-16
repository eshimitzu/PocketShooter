namespace Heyworks.PocketShooter.Meta.Communication
{
    /// <summary>
    /// Contains data required for player creation.
    /// </summary>
    public class CreatePlayerData
    {
        /// <summary>
        /// Gets or sets the player's nickname.
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// Gets or set a player's device id.
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// Gets or sets a player's group.
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Gets or sets the player's country.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the player's application bundle identifier.
        /// </summary>
        public string BundleId { get; set; }

        /// <summary>
        /// Gets or sets the current application store name.
        /// </summary>
        public ApplicationStoreName ApplicationStore { get; set; }

        /// <summary>
        /// Gets or sets a player's game client version.
        /// </summary>
        public string ClientVersion { get; set; }
    }
}
