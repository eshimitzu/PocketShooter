using Heyworks.PocketShooter.Meta.Entities;

namespace Heyworks.PocketShooter.Meta.Services
{
    /// <summary>
    /// Represents the result of connecting user to social account.
    /// </summary>
    public class SocialConnectResult
    {
        /// <summary>
        /// Gets or sets the social connect return code.
        /// </summary>
        public SocialConnectReturnCode ReturnCode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the social connection.
        /// </summary>
        public SocialConnection SocialConnection
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the user nickname belongs to device account.
        /// </summary>
        public string DeviceUserNickname
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the user nickname belongs to social account.
        /// </summary>
        public string SocialUserNickname
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the social account internal id.
        /// </summary>
        public string SocialId
        {
            get;
            set;
        }
    }

    /// <summary>
    /// This enumeration contains return codes for social connect operation.
    /// </summary>
    public enum SocialConnectReturnCode
    {
        /// <summary>
        /// Login success.
        /// </summary>
        Success = 0,

        /// <summary>
        /// Connect failed. User's social credentials are invalid.
        /// </summary>
        FailUserSocialCredentialsAreInvalid = 10,

        /// <summary>
        /// Connect was changed by another social account.
        /// </summary>
        FailSocialAccountConnectedToAnotherUser = 11,

        /// <summary>
        /// Connect failed. User is connected to another social account.
        /// </summary>
        FailUserConnectedToAnotherSocialAccount = 12,
    }
}
