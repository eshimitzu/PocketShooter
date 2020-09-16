namespace Heyworks.PocketShooter.Realtime.Data
{
    public class TrooperMetaInfo
    {
        /// <summary>
        /// Gets or sets a trooper's class.
        /// </summary>
        public TrooperClass Class { get; set; }

        /// <summary>
        /// Gets or sets the trooper's grade.
        /// </summary>
        public Grade Grade { get; set; }

        /// <summary>
        /// Gets or sets the trooper's level.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Gets or sets the max trooper's level for the current grade.
        /// </summary>
        public int MaxLevel { get; set; }

        /// <summary>
        /// Gets or sets the attack stat.
        /// </summary>
        public int Attack { get; set; }

        /// <summary>
        /// Gets or sets the health stat.
        /// </summary>
        public int Health { get; set; }

        /// <summary>
        /// Gets or sets the armor stat.
        /// </summary>
        public int Armor { get; set; }

        /// <summary>
        /// Gets or sets the movement stat.
        /// </summary>
        public double Movement { get; set; }

        /// <summary>
        /// Gets or sets the power stat.
        /// </summary>
        public int Power { get; set; }

        /// <summary>
        /// Gets or sets the trooper's max potential power.
        /// </summary>
        public int MaxPotentialPower { get; set; }

        /// <summary>
        /// Gets or sets the trooper's items info.
        /// </summary>
        public ItemsMetaInfo ItemsInfo { get; set; }
    }
}
