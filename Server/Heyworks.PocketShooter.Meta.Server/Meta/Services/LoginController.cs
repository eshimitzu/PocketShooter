using System;
using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Communication;
using Heyworks.PocketShooter.Meta.Entities;
using MongoDB.Driver;
using Orleans;

namespace Heyworks.PocketShooter.Meta.Services
{
    /// <summary>
    /// Abstract class that defines login flow.
    /// </summary>
    internal abstract class LoginController
    {
        private readonly IClusterClient clusterClient;
        private readonly IMongoCollection<User> userRepository;

        #region [Constructors and initialization]

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginController"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="clusterClient">The cluster client.</param>
        protected LoginController(IMongoCollection<User> userRepository, IClusterClient clusterClient)
        {
            this.clusterClient = clusterClient.NotNull();
            this.userRepository = userRepository.NotNull();
        }

        #endregion [Constructors and initialization]

        #region [Public interface]

        /// <summary>
        /// Performs user login to the system.
        /// </summary>
        /// <param name="loginProperties">The login properties.</param>
        /// <returns>An instance of <see cref="LoginResult"/>.</returns>
        public async Task<LoginResult> LoginAsync(LoginProperties loginProperties)
        {
            ILoginState loginState = await CheckCredentials(loginProperties);
            if (loginState.IsSuccess)
            {
                await ProceedLogin(loginState.User, loginProperties);
            }

            return loginState.CreateResult();
        }

        #endregion [Public interface]

        #region [Protected overridable members]

        /// <summary>
        /// Checks all login conditions before user login depending on the login account type - either device, or Google, or Facebook etc.
        /// </summary>
        /// <param name="properties">The login properties.</param>
        /// <returns>An instance of <see cref="ILoginState"/> implementation that contains all information of current user login state.</returns>
        protected abstract Task<ILoginState> CheckCredentials(LoginProperties properties);

        /// <summary>
        /// Updates user that's just logged in.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="loginProperties">The login properties.</param>
        protected virtual async Task ProceedLogin(User user, LoginProperties loginProperties)
        {
            var userId = Guid.Parse(user.Id);
            var playerGrain = clusterClient.GetGrain<IPlayerGrain>(userId);
            await playerGrain.UpdateClientData(loginProperties.BundleId, loginProperties.ApplicationStore, loginProperties.ClientVersion);

            var gameGrain = clusterClient.GetGrain<IGameGrain>(userId);
            await gameGrain.CheckRunnables();

            await userRepository.ReplaceOneAsync(_ => _.Id == user.Id, user);
        }

        /// <summary>
        /// Represents a state container shared among the all steps during login flow.
        /// </summary>
        protected interface ILoginState
        {
            /// <summary>
            /// Gets a value indicating whether a login condition check was successful.
            /// </summary>
            bool IsSuccess { get; }

            /// <summary>
            /// Gets an instance of <see cref="Entities.User"/> entity that just logged in.
            /// </summary>
            User User { get; }

            /// <summary>
            /// Creates <see cref="LoginResult"/> instance depending on the current login state.
            /// </summary>
            LoginResult CreateResult();
        }

        #endregion [Protected overridable members]
    }
}
