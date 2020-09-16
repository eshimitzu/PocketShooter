namespace Heyworks.PocketShooter.Meta.Services
{
    /// <summary>
    /// Represents user's social login result.
    /// </summary>
    public class SocialLoginResult : LoginResult
    {
        /// <summary>
        /// Gets or sets the social account internal id.
        /// </summary>
        public string SocialId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the original device account is switched to another social network-connected account.
        /// </summary>
        public bool IsAccountChanged
        {
            get;
            set;
        }
    }
}
