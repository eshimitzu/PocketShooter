using System;
using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Entities;
using MongoDB.Driver;
using Orleans;

namespace Heyworks.PocketShooter.Meta.Services
{
    /// <summary>
    /// Defines common parts of login flow for social network.
    /// </summary>
    /// <typeparam name="TConnection">The Social Connection type.</typeparam>
    internal abstract class SocialLoginController<TConnection> : LoginController
        where TConnection : SocialConnection
    {
        private readonly ISocialCredentialsValidator socialCredentialsValidator;

        #region [Constructors and initialization]

        /// <summary>
        /// Initializes a new instance of the <see cref="SocialLoginController{TConnection}"/> class.
        /// </summary>
        /// <param name="socialCredentialsValidator">The social credentials validator.</param>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="clusterClient">The cluster client.</param>
        protected SocialLoginController(
            ISocialCredentialsValidator socialCredentialsValidator,
            IMongoCollection<User> userRepository,
            IClusterClient clusterClient)
            : base(userRepository, clusterClient)
        {
            this.socialCredentialsValidator = socialCredentialsValidator.NotNull();
        }

        #endregion [Constructors and initialization]

        #region [Protected members]

        /// <summary>
        /// Checks all login conditions before user login depending on the login account type - either device, or Google, or Facebook etc.
        /// </summary>
        /// <param name="properties">The login properties.</param>
        /// <returns>An instance of <see cref="SocialLoginState"/> that contains all information of current user login state.</returns>
        protected override async Task<ILoginState> CheckCredentials(LoginProperties properties)
        {
            var socialLoginProperties = (SocialLoginProperties)properties;

            var socialLoginState = new SocialLoginState()
            {
                SocialId = socialLoginProperties.SocialId,
            };

            User userToLogIn = await FindUserBySocialId(socialLoginProperties.SocialId);

            if (userToLogIn == null)
            {
                socialLoginState.ReturnCode = LoginReturnCode.FailUserNotFound;
                return socialLoginState;
            }

            SocialConnection connection = userToLogIn.SocialConnections.GetConnection(typeof(TConnection));
            bool validationResult = false;

            // If login token is received - try to login via token
            if (socialLoginProperties.HasLoginToken)
            {
                validationResult = ValidateLoginToken(connection, socialLoginProperties.LoginToken);
                if (!validationResult)
                {
                    // clear login token if invalid. We will generate a new one later in UpdateSocialConnection.
                    socialLoginProperties.LoginToken = null;
                }
            }

            // Otherwise login via social credentials
            if (!validationResult)
            {
                validationResult = await socialCredentialsValidator.ValidateLoginAsync(userToLogIn, socialLoginProperties);
            }

            if (validationResult)
            {
                socialLoginState.ReturnCode = LoginReturnCode.Success;

                socialLoginState.IsAccountChanged = userToLogIn.DeviceId != socialLoginProperties.DeviceId;
                socialLoginState.User = userToLogIn;
            }
            else
            {
                socialLoginState.ReturnCode = LoginReturnCode.FailUserSocialCredentialsAreInvalid;
            }

            return socialLoginState;
        }

        /// <summary>
        /// Updates user that's just logged in.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="loginProperties">The login properties.</param>
        protected override async Task ProceedLogin(User user, LoginProperties loginProperties)
        {
            await base.ProceedLogin(user, loginProperties);

            var socialLoginProperties = (SocialLoginProperties)loginProperties;
            UpdateSocialConnection(user, socialLoginProperties);
        }

        /// <summary>
        /// Finds user by given social id.
        /// </summary>
        /// <param name="socialInternalId">The social account internal id.</param>
        protected abstract Task<User> FindUserBySocialId(string socialInternalId);

        /// <summary>
        /// Updates social connection instance after user login.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="socialLoginProperties">The social connect properties.</param>
        protected virtual void UpdateSocialConnection(User user, SocialLoginProperties socialLoginProperties)
        {
            var connection = user.SocialConnections.GetConnection(typeof(TConnection));

            connection.SetLastActivity(DateTime.UtcNow);

            // Refresh social login token if login was made with social credentials or login token is invalid.
            if (!socialLoginProperties.HasLoginToken)
            {
                connection.RefreshLoginToken();
            }
        }

        /// <summary>
        /// Represents the social login state container.
        /// </summary>
        protected class SocialLoginState : ILoginState
        {
            /// <summary>
            /// Gets or sets the login return code.
            /// </summary>
            public LoginReturnCode ReturnCode
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the social internal id.
            /// </summary>
            public string SocialId
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets a value indicating whether the original device account is switched to another social network-connected account.
            /// </summary>
            public bool IsAccountChanged
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
            /// Gets or sets an instance of <see cref="Entities.User"/> entity that just logged in.
            /// </summary>
            public User User
            {
                get;
                set;
            }

            /// <summary>
            /// Creates <see cref="LoginResult"/> instance depending on the current login state.
            /// </summary>
            public LoginResult CreateResult()
            {
                return new SocialLoginResult()
                {
                    User = User,
                    ReturnCode = ReturnCode,
                    SocialId = SocialId,
                    IsAccountChanged = IsAccountChanged,
                };
            }
        }

        #endregion [Protected members]

        #region [Private members]

        private static bool ValidateLoginToken(SocialConnection connection, string clientLoginToken)
        {
            return
                connection.LoginTokenExpirationDate >= DateTime.UtcNow &&
                connection.LoginToken == clientLoginToken;
        }

        #endregion [Private members]
    }
}
