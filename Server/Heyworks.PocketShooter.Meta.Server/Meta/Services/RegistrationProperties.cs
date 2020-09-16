namespace Heyworks.PocketShooter.Meta.Services
{
    /// <summary>
    /// Represents a container for data needed for a new user registration.
    /// </summary>
    public class RegistrationProperties : LoginProperties
    {
        /// <summary>
        /// Gets or sets the user's country name.
        /// </summary>
        public string Country
        {
            get;
            set;
        }
    }
}
