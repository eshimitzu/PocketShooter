using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Entities;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Orleans;

namespace Heyworks.PocketShooter.Meta.Services
{
    /// <summary>
    /// Represents service responsible for user registration, login and social connect.
    /// </summary>
    internal class AccountService : IAccountService
    {
        private readonly IMongoCollection<User> userRepository;
        private readonly GoogleCredentialsValidator googleCredentialsValidator;
        private readonly IClusterClient clusterClient;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountService"/> class.
        /// </summary>
        /// <param name="userRepository">The player repository.</param>
        /// <param name="googleCredentialsValidator">The Google credentials validator.</param>
        /// <param name="clusterClient">The cluster client.</param>
        /// <param name="logger">The logger.</param>
        public AccountService(
            IMongoCollection<User> userRepository,
            GoogleCredentialsValidator googleCredentialsValidator,
            IClusterClient clusterClient,
            ILogger<AccountService> logger)
        {
            this.userRepository = userRepository;
            this.googleCredentialsValidator = googleCredentialsValidator;
            this.clusterClient = clusterClient;
            this.logger = logger;
        }

        /// <summary>
        /// Try to register a new user by his device id.
        /// </summary>
        /// <param name="properties">The registration properties.</param>
        public Task<RegistrationResult> RegisterWithDeviceAsync(RegistrationProperties properties)
        {
            var controller = new DeviceRegistrationController(userRepository, clusterClient, logger);
            return controller.RegisterAsync(properties);
        }

        /// <summary>
        /// Try to login user by his device id.
        /// </summary>
        /// <param name="loginProperties">The login properties.</param>
        public Task<LoginResult> LoginWithDeviceAsync(LoginProperties loginProperties)
        {
            var controller = new DeviceLoginController(userRepository, clusterClient);
            return controller.LoginAsync(loginProperties);
        }

        /// <summary>
        /// Try to login user via Google account.
        /// </summary>
        /// <param name="loginProperties">The Google login properties.</param>
        public Task<LoginResult> LoginWithGoogleAsync(GoogleLoginProperties loginProperties)
        {
            var controller = new GoogleLoginController(googleCredentialsValidator, userRepository, clusterClient);
            return controller.LoginAsync(loginProperties);
        }

        /// <summary>
        /// Connect existing device account to a Google account. An error is occurred if device account is already connected to another Google account or the given Google account
        /// has been already connected to another device army.
        /// </summary>
        /// <param name="userId">The current user id.</param>
        /// <param name="connectProperties">The Google social connect properties object.</param>
        public async Task<SocialConnectResult> ConnectGoogleAsync(string userId, GoogleConnectProperties connectProperties)
        {
            var user = await userRepository.Find(_ => _.Id == userId).FirstAsync();

            var controller = new GoogleConnectController(googleCredentialsValidator, userRepository, clusterClient);

            return await controller.ConnectAsync(user, connectProperties);
        }
    }
}
