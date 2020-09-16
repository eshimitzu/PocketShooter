using Heyworks.PocketShooter.Meta.Data;

namespace Heyworks.PocketShooter.Meta.Communication
{
    /// <summary>
    /// Represents data for social login operation response.
    /// </summary>
    public class SocialLoginResponseData : LoginResponseData
    {
        /// <summary>
        /// Gets or sets the social connection state.
        /// </summary>
        public SocialConnectionState SocialConnection
        {
            get;
            set;
        }
    }
}
