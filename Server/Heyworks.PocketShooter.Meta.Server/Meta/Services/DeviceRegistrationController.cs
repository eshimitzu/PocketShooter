using System;
using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Communication;
using Heyworks.PocketShooter.Meta.Entities;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Orleans;

namespace Heyworks.PocketShooter.Meta.Services
{
    /// <summary>
    /// Defines device id registration flow.
    /// </summary>
    internal class DeviceRegistrationController : RegistrationController
    {
        private readonly IMongoCollection<User> userRepository;
        private readonly IClusterClient clusterClient;

        #region [Constructors and initialization]

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceRegistrationController"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="clusterClient">The cluster client.</param>
        /// <param name="logger">The logger.</param>
        public DeviceRegistrationController(IMongoCollection<User> userRepository, IClusterClient clusterClient, ILogger logger)
            : base(userRepository, clusterClient, logger)
        {
            this.userRepository = userRepository.NotNull();
            this.clusterClient = clusterClient;
        }

        #endregion [Constructors and initialization]

        #region [Protected members]

        /// <summary>
        /// Checks that user with the same device id does not exist.
        /// </summary>
        /// <param name="properties">The registration properties.</param>
        /// <returns>True if user does not exist and it is possible to register a new user.</returns>
        protected override async Task<IRegistrationState> CheckUserDoesNotExist(RegistrationProperties properties)
        {
            User existingUser = await userRepository.Find(entity => entity.DeviceId == properties.DeviceId).FirstOrDefaultAsync();

            return
                existingUser != null
                ? new DeviceRegistrationState(RegistrationReturnCode.FailDeviceIdExists, clusterClient)
                : new DeviceRegistrationState(RegistrationReturnCode.Success, clusterClient);
        }

        /// <summary>
        /// Represents the device registration state container.
        /// </summary>
        private class DeviceRegistrationState : IRegistrationState
        {
            private readonly IClusterClient clusterClient;

            /// <summary>
            /// Initializes a new instance of the <see cref="DeviceRegistrationState"/> class with return code.
            /// </summary>
            /// <param name="returnCode">The registration return code.</param>
            public DeviceRegistrationState(RegistrationReturnCode returnCode, IClusterClient clusterClient)
            {
                ReturnCode = returnCode;
                this.clusterClient = clusterClient;
            }

            /// <summary>
            /// Gets a value indicating whether a registration condition check was successful.
            /// </summary>
            public bool IsSuccess => ReturnCode == RegistrationReturnCode.Success;

            /// <summary>
            /// Gets the registration return code.
            /// </summary>
            public RegistrationReturnCode ReturnCode
            {
                get;
                private set;
            }

            /// <summary>
            /// Creates <see cref="RegistrationResult"/> instance depending on the current registration state.
            /// </summary>
            /// <param name="registeredUser">The user that is just registered.</param>
            /// <param name="userGroup">The user group.</param>
            public async Task<RegistrationResult> CreateResult(User registeredUser, string userGroup)
            {
                var configGrain = clusterClient.GetGrain<IConfigGrain>(Guid.Empty);
                var gameConfigVersion = await configGrain.GetGameConfigVersion(userGroup);

                return new RegistrationResult()
                {
                    User = registeredUser,
                    GameConfigVersion = gameConfigVersion,
                    ReturnCode = ReturnCode,
                };
            }

            /// <summary>
            /// Creates <see cref="RegistrationResult"/> instance depending on the current registration state.
            /// </summary>
            public Task<RegistrationResult> CreateResult() =>
                Task.FromResult(new RegistrationResult
                {
                    ReturnCode = ReturnCode,
                });
        }

        #endregion [Protected members]
    }
}
