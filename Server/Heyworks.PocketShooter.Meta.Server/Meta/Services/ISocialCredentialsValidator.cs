using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Entities;

namespace Heyworks.PocketShooter.Meta.Services
{
    /// <summary>
    /// Represents component which can validate social account credentials.
    /// </summary>
    internal interface ISocialCredentialsValidator
    {
        /// <summary>
        /// Validates social credentials during connecting the existing account to some social account.
        /// Returns true if validation was successful.
        /// </summary>
        /// <param name="userToConnect">The user to connect social account.</param>
        /// <param name="connectProperties">The connection properties.</param>
        Task<bool> ValidateConnectAsync(User userToConnect, SocialConnectProperties connectProperties);

        /// <summary>
        /// Validates social credentials during connecting the existing account to some social account.
        /// </summary>
        /// <param name="userToLogin">The user to login.</param>
        /// <param name="loginProperties">The login properties.</param>
        Task<bool> ValidateLoginAsync(User userToLogin, SocialLoginProperties loginProperties);
    }
}
