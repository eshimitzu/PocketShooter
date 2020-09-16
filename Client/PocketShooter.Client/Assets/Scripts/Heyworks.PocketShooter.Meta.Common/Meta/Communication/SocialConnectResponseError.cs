namespace Heyworks.PocketShooter.Meta.Communication
{
    /// <summary>
    /// Data server response error class for social connect operation.
    /// </summary>
    public class SocialConnectResponseError : ResponseError
    {
        public SocialConnectResponseError()
        {
        }

        public SocialConnectResponseError(ApiErrorCode code)
            : base(code)
        {
        }

        public SocialConnectResponseError(ApiErrorCode code, string message)
            : base(code, message)
        {
        }

        /// <summary>
        /// Gets or sets the device user nickname. Required for generating proper error messages during social connect.
        /// </summary>
        public string DeviceUserNickname
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the social user nickname. Required for generating proper error messages during social connect.
        /// </summary>
        public string SocialUserNickname
        {
            get;
            set;
        }
    }
}
