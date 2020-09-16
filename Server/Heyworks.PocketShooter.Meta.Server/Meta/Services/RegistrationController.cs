using System;
using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Communication;
using Heyworks.PocketShooter.Meta.Entities;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Orleans;
using Orleans.Concurrency;

namespace Heyworks.PocketShooter.Meta.Services
{
    /// <summary>
    /// Defines abstract registration flow.
    /// </summary>
    internal abstract class RegistrationController
    {
        private readonly IMongoCollection<User> userRepository;
        private readonly IClusterClient clusterClient;
        private readonly ILogger logger;

        #region [Constructors and initialization]

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationController"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="clusterClient">The cluster client.</param>
        /// <param name="logger">The logger.</param>
        protected RegistrationController(IMongoCollection<User> userRepository, IClusterClient clusterClient, ILogger logger)
        {
            this.userRepository = userRepository.NotNull();
            this.clusterClient = clusterClient.NotNull();
            this.logger = logger;
        }

        #endregion [Constructors and initialization]

        #region [Public interface]

        /// <summary>
        /// Registers a new user in the system using data extracted from <see cref="RegistrationProperties"/>.
        /// </summary>
        /// <param name="properties">The registration properties.</param>
        /// <returns>An instance of <see cref="RegistrationResult"/>.</returns>
        public async Task<RegistrationResult> RegisterAsync(RegistrationProperties properties)
        {
            IRegistrationState registrationState = await CheckUserDoesNotExist(properties);
            RegistrationResult result;

            if (registrationState.IsSuccess)
            {
                var newUser = await ProceedRegister(properties, registrationState);
                result = await registrationState.CreateResult(newUser.User, newUser.Group);
            }
            else
            {
                result = await registrationState.CreateResult();
            }

            return result;
        }

        #endregion [Public interface]

        #region [Protected overridable members]

        /// <summary>
        /// Checks that user with the same account credentials does not exist.
        /// </summary>
        /// <param name="properties">The registration properties.</param>
        /// <returns>True if user does not exist and it is possible to register a new user.</returns>
        protected abstract Task<IRegistrationState> CheckUserDoesNotExist(RegistrationProperties properties);

        /// <summary>
        /// Creates a new instance of the <see cref="User"/> and initializes it with <see cref="RegistrationProperties"/>.
        /// </summary>
        /// <param name="properties">The registration properties.</param>
        /// <param name="state">The registration state.</param>
        protected virtual async Task<(User User, string Group)> ProceedRegister(RegistrationProperties properties, IRegistrationState state)
        {
            var user = new User(Guid.NewGuid().ToString(), properties.DeviceId);

            await userRepository.InsertOneAsync(user);

            var group = GetGroup();

            var gameGrain = clusterClient.GetGrain<IGameGrain>(Guid.Parse(user.Id));
            var playerData = new CreatePlayerData
            {
                Nickname = GenerateNickname(),
                DeviceId = properties.DeviceId,
                Group = group,
                Country = properties.Country,
                BundleId = properties.BundleId,
                ApplicationStore = properties.ApplicationStore,
                ClientVersion = properties.ClientVersion,
            };

            await gameGrain.CreatePlayerGame(playerData.AsImmutable());

            logger.LogInformation(
                "Player id={id}, deviceId={deviceId}, nickname={nickname}, country={country}, store={store} was created in system",
                user.Id,
                user.DeviceId,
                playerData.Nickname,
                playerData.Country,
                playerData.ApplicationStore);

            return (user, group);
        }

        /// <summary>
        /// Represents a state container shared among the all steps during registration flow.
        /// </summary>
        protected interface IRegistrationState
        {
            /// <summary>
            /// Gets a value indicating whether a registration condition check was successful.
            /// </summary>
            bool IsSuccess { get; }

            /// <summary>
            /// Creates <see cref="RegistrationResult"/> instance depending on the current registration state.
            /// </summary>
            /// <param name="registeredUser">The user that is just registered.</param>
            /// <param name="userGroup">The user group.</param>
            Task<RegistrationResult> CreateResult(User registeredUser, string userGroup);

            /// <summary>
            /// Creates <see cref="RegistrationResult"/> instance depending on the current registration state.
            /// </summary>
            Task<RegistrationResult> CreateResult();
        }

        #endregion [Protected overridable members]

        private static string GenerateNickname() => "User" + DateTime.UtcNow.Millisecond;

        // TODO: must do more complex segregation for A/B testing, e.g `experimental` for some test group against normal users and put them into own matches
        private static string GetGroup() => "default";
    }
}
