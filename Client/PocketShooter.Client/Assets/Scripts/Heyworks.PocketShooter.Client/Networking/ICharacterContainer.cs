using System.Collections.Generic;
using Heyworks.PocketShooter.Networking.Actors;

namespace Heyworks.PocketShooter.Networking
{
    /// <summary>
    /// Represents characters container.
    /// </summary>
    public interface ICharacterContainer
    {
        /// <summary>
        /// Gets the character by player id. Returns null if not found.
        /// </summary>
        /// <param name="playerId">The player identifier.</param>
        NetworkCharacter GetCharacter(int playerId);

        /// <summary>
        /// Gets the characters list presented in the game.
        /// </summary>
        IEnumerable<NetworkCharacter> Characters { get; }

        /// <summary>
        /// Returns all characters inside the circle with radius.
        /// </summary>
        /// <param name="oX">X coord of the center.</param>
        /// <param name="oZ">Z coord of the center.</param>
        /// <param name="radiusSqr">Square radius of the zone.</param>
        IEnumerable<NetworkCharacter> GetCharactersInsideCircle(float oX, float oZ, float radiusSqr);

        /// <summary>
        /// Returns all characters inside the cylinder with radius and height.
        /// </summary>
        /// <param name="oX">X coord of the center.</param>
        /// <param name="oY">Y coord of the center.</param>
        /// <param name="oZ">Z coord of the center.</param>
        /// <param name="radiusSqr">Square radius of the zone.</param>
        /// <param name="height">Height of the zone.</param>
        IEnumerable<NetworkCharacter> GetCharactersInsideCylinder(
            float oX,
            float oY,
            float oZ,
            float radiusSqr,
            float height);
    }
}