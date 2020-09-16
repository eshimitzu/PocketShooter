using Heyworks.PocketShooter.Meta.Entities;

namespace Heyworks.PocketShooter.Meta.Services
{
    /// <summary>
    /// Represents user's login result.
    /// </summary>
    public class LoginResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginResult"/> class.
        /// </summary>
        protected LoginResult()
        {
        }

        /// <summary>
        /// Gets or sets the authenticated user.
        /// </summary>
        public User User
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the login return code.
        /// </summary>
        public LoginReturnCode ReturnCode
        {
            get;
            set;
        }
    }

    /// <summary>
    /// This enumeration contains return codes for login operation.
    /// </summary>
    public enum LoginReturnCode
    {
        /// <summary>
        /// Login success.
        /// </summary>
        Success = 0,

        /// <summary>
        /// Login failed.
        /// </summary>
        FailUserNotFound = 10,

        /// <summary>
        /// Login failed. User's social credentials are invalid.
        /// </summary>
        FailUserSocialCredentialsAreInvalid = 11,
    }
}
