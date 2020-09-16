using System;
using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Entities;
using MongoDB.Driver;
using Orleans;

namespace Heyworks.PocketShooter.Meta.Services
{
    /// <summary>
    /// Defines device id login flow.
    /// </summary>
    internal class DeviceLoginController : LoginController
    {
        private readonly IMongoCollection<User> userRepository;

        #region [Constructors and initialization]

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceLoginController"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="clusterClient">The cluster client.</param>
        public DeviceLoginController(IMongoCollection<User> userRepository, IClusterClient clusterClient)
            : base(userRepository, clusterClient)
        {
            this.userRepository = userRepository.NotNull();
        }

        #endregion [Constructors and initialization]

        #region [Protected members]

        /// <summary>
        /// Checks all login conditions before user login depending on the login account type - either device, or Google, or Facebook etc.
        /// </summary>
        /// <param name="properties">The login properties.</param>
        /// <returns>An instance of <see cref="LoginController.ILoginState"/>.</returns>
        protected override async Task<ILoginState> CheckCredentials(LoginProperties properties)
        {
            var deviceLoginState = new DeviceLoginState
            {
                DeviceId = properties.DeviceId,
            };

            var user = await userRepository.Find(_ => _.DeviceId == properties.DeviceId).FirstOrDefaultAsync();

            deviceLoginState.ReturnCode = user != null ? LoginReturnCode.Success : LoginReturnCode.FailUserNotFound;
            deviceLoginState.User = user;

            return deviceLoginState;
        }

        /// <summary>
        /// Represents the device login state container.
        /// </summary>
        private class DeviceLoginState : ILoginState
        {
            /// <summary>
            /// Gets or sets an instance of <see cref="Entities.User"/> entity that just logged in.
            /// </summary>
            public User User
            {
                get;
                set;
            }

            /// <summary>
            /// Gets a value indicating whether a login condition check was successful.
            /// </summary>
            public bool IsSuccess
            {
                get
                {
                    return User != null;
                }
            }

            /// <summary>
            /// Creates <see cref="LoginResult"/> instance depending on the current login state.
            /// </summary>
            public LoginResult CreateResult()
            {
                return new DeviceLoginResult()
                {
                    User = User,
                    ReturnCode = ReturnCode,
                    DeviceId = DeviceId,
                };
            }

            /// <summary>
            /// Gets or sets the login return code.
            /// </summary>
            public LoginReturnCode ReturnCode
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the device id.
            /// </summary>
            public string DeviceId
            {
                get;
                set;
            }
        }

        #endregion [Protected members]
    }
}
