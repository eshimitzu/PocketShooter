using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Entities;

namespace Heyworks.PocketShooter.Meta.Services
{
    /// <summary>
    /// Represents service responsible for user registration, login and social connect.
    /// </summary>
    public interface IAccountService
    {
        /// <summary>
        /// Try to register a new user by his device id.
        /// </summary>
        /// <param name="properties">The registration properties.</param>
        Task<RegistrationResult> RegisterWithDeviceAsync(RegistrationProperties properties);

        /// <summary>
        /// Try to login user by his device id.
        /// </summary>
        /// <param name="properties">The login properties.</param>
        Task<LoginResult> LoginWithDeviceAsync(LoginProperties properties);

        /// <summary>
        /// Try to login user via Google account.
        /// </summary>
        /// <param name="properties">The Google login properties.</param>
        Task<LoginResult> LoginWithGoogleAsync(GoogleLoginProperties properties);

        /// <summary>
        /// Connect existing device account to a Google account. An error is occurred if device account is already connected to another Google account or the given Google account
        /// has been already connected to another device army.
        /// </summary>
        /// <param name="userId">The current user id.</param>
        /// <param name="properties">The Google social connect properties object.</param>
        Task<SocialConnectResult> ConnectGoogleAsync(string userId, GoogleConnectProperties properties);
    }
}
