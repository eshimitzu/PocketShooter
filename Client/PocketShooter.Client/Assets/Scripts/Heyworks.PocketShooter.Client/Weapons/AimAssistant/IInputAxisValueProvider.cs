namespace Heyworks.PocketShooter.Weapons.AimAssistant
{
    /// <summary>
    /// Provides the interface for obtaining input values on axes.
    /// </summary>
    public interface IInputAxisValueProvider
    {
        /// <summary>
        /// Gets the value on mouse X axis.
        /// </summary>
        float MouseX { get; }

        /// <summary>
        /// Gets the value on mouse Y axis.
        /// </summary>
        float MouseY { get; }
    }
}