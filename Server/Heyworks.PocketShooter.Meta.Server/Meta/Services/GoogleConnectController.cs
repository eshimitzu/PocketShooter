using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Entities;
using MongoDB.Driver;
using Orleans;

namespace Heyworks.PocketShooter.Meta.Services
{
    /// <summary>
    /// Controller class that is responsible for connect and relink of Google social accounts.
    /// </summary>
    internal class GoogleConnectController : SocialConnectController<GoogleConnection>
    {
        private readonly IMongoCollection<User> userRepository;

        #region [Constructors & initialization]

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleConnectController"/> class.
        /// </summary>
        /// <param name="credentialsValidator">The credentials validator.</param>
        /// <param name="userRepository">The user repository.</param>
        public GoogleConnectController(
            GoogleCredentialsValidator credentialsValidator,
            IMongoCollection<User> userRepository,
            IClusterClient clusterClient)
            : base(credentialsValidator, userRepository, clusterClient)
        {
            this.userRepository = userRepository;
        }

        #endregion [Constructors & initialization]

        #region [Protected members]

        /// <summary>
        /// Finds user by given social id.
        /// </summary>
        /// <param name="socialInternalId">The social account internal id.</param>
        protected override Task<User> FindUserBySocialId(string socialInternalId) =>
            userRepository
            .Find(entity => entity.SocialConnections.Google.InternalId == socialInternalId)
            .FirstOrDefaultAsync();

        /// <summary>
        /// Creates and initializes a new Google social connection.
        /// </summary>
        /// <param name="connectProperties">The social connect properties.</param>
        protected override GoogleConnection CreateSocialConnection(SocialConnectProperties connectProperties) =>
            new GoogleConnection(connectProperties.SocialId);

        #endregion [Protected members]
    }
}
