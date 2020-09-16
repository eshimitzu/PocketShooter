using System;
using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Entities;
using MongoDB.Driver;
using Orleans;

namespace Heyworks.PocketShooter.Meta.Services
{
    /// <summary>
    /// Class that defines Google login flow.
    /// </summary>
    internal class GoogleLoginController : SocialLoginController<GoogleConnection>
    {
        private readonly IMongoCollection<User> userRepository;

        #region [Constructors and initialization]

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleLoginController"/> class.
        /// </summary>
        /// <param name="credentialsValidator">The credentials validator.</param>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="clusterClient">The cluster client.</param>
        public GoogleLoginController(
            GoogleCredentialsValidator credentialsValidator,
            IMongoCollection<User> userRepository,
            IClusterClient clusterClient)
            : base(credentialsValidator, userRepository, clusterClient)
        {
            this.userRepository = userRepository.NotNull();
        }

        #endregion [Constructors and initialization]

        #region [Protected members]

        /// <summary>
        /// Finds user by given social id.
        /// </summary>
        /// <param name="socialInternalId">The social account internal id.</param>
        protected override Task<User> FindUserBySocialId(string socialInternalId) =>
            userRepository
            .Find(entity => entity.SocialConnections.Google.InternalId == socialInternalId)
            .FirstOrDefaultAsync();

        #endregion [Protected members]
    }
}
