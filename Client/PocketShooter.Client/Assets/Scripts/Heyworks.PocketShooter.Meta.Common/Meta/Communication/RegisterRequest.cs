namespace Heyworks.PocketShooter.Meta.Communication
{
    /// <summary>
    /// Contains fields coming with the register request.
    /// </summary>
    public class RegisterRequest : LoginRequest
    {
        /// <summary>
        /// Gets or sets the player's country.
        /// </summary>
        public string Country { get; set; }
    }
}
