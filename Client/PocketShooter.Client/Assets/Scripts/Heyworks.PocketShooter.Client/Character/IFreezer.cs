namespace Heyworks.PocketShooter.Character
{
    /// <summary>
    /// Interface for objects affecting on player mobility.
    /// </summary>
    public interface IFreezer
    {
        /// <summary>
        /// Whether the player can move.
        /// </summary>
        bool FreezeMotion { get; }

        /// <summary>
        /// Whether the player can rotate.
        /// </summary>
        bool FreezeRotation { get; }
    }
}