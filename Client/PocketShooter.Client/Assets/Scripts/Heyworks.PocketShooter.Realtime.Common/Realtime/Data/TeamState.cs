using Heyworks.Realtime.State;

namespace Heyworks.PocketShooter.Realtime.Data
{
    public struct TeamState
    {
        /// <summary>
        /// Gets the team number.
        /// </summary>
        public readonly TeamNo Number;

        /// <summary>
        /// Gets the team score.
        /// </summary>
        public int Score;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamState"/> struct.
        /// </summary>
        /// <param name="number">Team number.</param>
        /// <param name="score">Score.</param>
        public TeamState(TeamNo number, int score)
        {
            Number = number;
            Score = score;
        }

        /// <summary>
        /// Returns a description that represents the current <see cref="TeamState"/>.
        /// </summary>
        /// <returns>A description that represents the current <see cref="TeamState"/>.</returns>
        public override string ToString() =>
            $"number: {Number} : score: {Score}";
    }

    /// <summary>
    /// Team State Extensions.
    /// </summary>
    public static class TeamStateExtensions
    {
        public static void Clone(this in TeamState self, ref TeamState to) => UnsafeClone.Clone(in self, ref to);
    }
}