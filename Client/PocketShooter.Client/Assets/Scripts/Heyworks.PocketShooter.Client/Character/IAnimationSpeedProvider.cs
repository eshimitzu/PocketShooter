namespace Heyworks.PocketShooter.Character
{
    /// <summary>
    /// Represents interface for providing speed for animation controller.
    /// </summary>
    public interface IAnimationSpeedProvider
    {
        /// <summary>
        /// Gets the current speed along forward axis.
        /// </summary>
        float CurrentForwardSpeed { get; }

        /// <summary>
        /// Gets the current speed along right axis.
        /// </summary>
        float CurrentRightSpeed { get; }
    }
}
