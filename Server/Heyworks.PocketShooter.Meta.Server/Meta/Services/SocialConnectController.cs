using System;
using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Communication;
using Heyworks.PocketShooter.Meta.Entities;
using MongoDB.Driver;
using Orleans;

namespace Heyworks.PocketShooter.Meta.Services
{
    /// <summary>
    /// Controller class that is responsible for connect and relink of social accounts
    /// </summary>
    /// <typeparam name="TConnection">The Social Connection type.</typeparam>
    internal abstract class SocialConnectController<TConnection>
        where TConnection : SocialConnection
    {
        private readonly ISocialCredentialsValidator socialCredentialsValidator;
        private readonly IMongoCollection<User> userRepository;
        private readonly IClusterClient clusterClient;

        #region [Constructors and initialization]

        /// <summary>
        /// Initializes a new instance of the <see cref="SocialConnectController{TConnection}"/> class.
        /// </summary>
        /// <param name="socialCredentialsValidator">The social credentials validator.</param>
        /// <param name="userRepository">The user repository.</param>
        protected SocialConnectController(
            ISocialCredentialsValidator socialCredentialsValidator,
            IMongoCollection<User> userRepository,
            IClusterClient clusterClient)
        {
            this.socialCredentialsValidator = socialCredentialsValidator;
            this.userRepository = userRepository;
            this.clusterClient = clusterClient;
        }

        #endregion [Constructors and initialization]

        #region [Public members]

        /// <summary>
        /// Connect existing device account to social account.
        /// Checks if device account is already connected to another social account or the given social account
        /// has been already connected to another device army.
        /// </summary>
        /// <param name="user">The current user.</param>
        /// <param name="connectProperties">The social connect properties.</param>
        public async Task<SocialConnectResult> ConnectAsync(User user, SocialConnectProperties connectProperties)
        {
            var result = new SocialConnectResult();

            var devicePlayerGrain = clusterClient.GetGrain<IPlayerGrain>(Guid.Parse(user.Id));
            result.DeviceUserNickname = await devicePlayerGrain.GetNickname();
            result.SocialId = connectProperties.SocialId;

            User socialAccountUser = await FindUserBySocialId(connectProperties.SocialId);
            if (socialAccountUser != null)
            {
                var socialPlayerGrain = clusterClient.GetGrain<IPlayerGrain>(Guid.Parse(socialAccountUser.Id));
                result.SocialUserNickname = await socialPlayerGrain.GetNickname();
            }

            // Case #1. Social account connected to some other user in the database.
            // Outcome: return error code
            if (socialAccountUser != null && socialAccountUser.Id != user.Id)
            {
                result.ReturnCode = SocialConnectReturnCode.FailSocialAccountConnectedToAnotherUser; // Switch
                result.SocialConnection = user.SocialConnections.GetConnection(typeof(TConnection));
                return result;
            }

            // Case #2. Current device user is already connected to another social account.
            // Outcome: return error code
            if (socialAccountUser == null && user.SocialConnections.HasConnection(typeof(TConnection)))
            {
                result.ReturnCode = SocialConnectReturnCode.FailUserConnectedToAnotherSocialAccount; // Skip
                return result;
            }

            // Case #3. Current user has no corresponding social connection and the given social id is not connected to any other user.
            // Actions: Verify social credentials, Create connection, Update connection.
            // Outcome: Success

            // Case #4. Current user is already connected the same social account that is going to be connected.
            // Actions: Verify social credentials, Update connection.
            // Outcome: Success

            // Verify social credentials
            if (!await socialCredentialsValidator.ValidateConnectAsync(user, connectProperties))
            {
                result.ReturnCode = SocialConnectReturnCode.FailUserSocialCredentialsAreInvalid;
                return result;
            }

            if (!user.SocialConnections.HasConnection(typeof(TConnection)))
            {
                SocialConnection socialConnection = CreateSocialConnection(connectProperties);
                user.SocialConnections.Connect(socialConnection);
            }

            UpdateSocialConnection((TConnection)user.SocialConnections.GetConnection(typeof(TConnection)), connectProperties);

            await userRepository.UpdateOneAsync(
                _ => _.Id == user.Id,
                Builders<User>.Update.Set(entity => entity.SocialConnections, user.SocialConnections));

            result.ReturnCode = SocialConnectReturnCode.Success;
            result.SocialConnection = user.SocialConnections.GetConnection(typeof(TConnection));

            return result;
        }

        #endregion [Public members]

        #region [Protected members]

        /// <summary>
        /// Finds user by given social id.
        /// </summary>
        /// <param name="socialInternalId">The social account internal id.</param>
        protected abstract Task<User> FindUserBySocialId(string socialInternalId);

        /// <summary>
        /// Creates and initializes a new social connection.
        /// </summary>
        /// <param name="connectProperties">The social connect properties.</param>
        protected abstract TConnection CreateSocialConnection(SocialConnectProperties connectProperties);

        /// <summary>
        /// Updates social connection state with data transferred from client.
        /// The data to update: friends list, access tokens, last activity.
        /// </summary>
        /// <param name="connection">The social connection to update.</param>
        /// <param name="connectProperties">The relink properties container.</param>
        private void UpdateSocialConnection(TConnection connection, SocialConnectProperties connectProperties)
        {
            connection.SetLastActivity(DateTime.UtcNow);
            connection.RefreshLoginToken();
        }

        #endregion [Protected members]
    }
}
