namespace Heyworks.PocketShooter.Character
{
    /// <summary>
    /// Represents the interface for input sensetivity multiplier provider.
    /// </summary>
    public interface IInputSensetivityProvider
    {
        /// <summary>
        /// Gets the input sensetivity multiplier.
        /// </summary>
        float InputSensetivityMultiplier { get; }
    }
}