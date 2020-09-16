namespace Heyworks.PocketShooter.Realtime.Data
{
    /// <summary>
    /// Spawn trooper command data.
    /// </summary>
    public readonly struct SpawnTrooperCommandData : IServiceCommandData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpawnTrooperCommandData"/> struct.
        /// </summary>
        /// <param name="trooperClass">The trooper class.</param>
        public SpawnTrooperCommandData(TrooperClass trooperClass)
        {
            TrooperClass = trooperClass;
        }

        /// <summary>
        /// Gets the trooper's class.
        /// </summary>
        public readonly TrooperClass TrooperClass;
    }
}
