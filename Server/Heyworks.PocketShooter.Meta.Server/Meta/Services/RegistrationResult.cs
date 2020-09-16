using System;
using Heyworks.PocketShooter.Meta.Entities;

namespace Heyworks.PocketShooter.Meta.Services
{
    /// <summary>
    /// Represents user registration result.
    /// </summary>
    public class RegistrationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationResult"/> class.
        /// </summary>
        public RegistrationResult()
        {
        }

        /// <summary>
        /// Gets or sets the registered user.
        /// </summary>
        public User User
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a game configuration version.
        /// </summary>
        public Version GameConfigVersion
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the registration return code.
        /// </summary>
        public RegistrationReturnCode ReturnCode
        {
            get;
            set;
        }
    }

    /// <summary>
    /// This enumeration contains return codes for registration operation.
    /// </summary>
    public enum RegistrationReturnCode
    {
        /// <summary>
        /// Registration success.
        /// </summary>
        Success = 0,

        /// <summary>
        /// Registration failed. User with the same device id already exists.
        /// </summary>
        FailDeviceIdExists = 1,
    }
}
