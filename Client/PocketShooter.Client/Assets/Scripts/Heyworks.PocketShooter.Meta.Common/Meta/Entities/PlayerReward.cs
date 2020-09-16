namespace Heyworks.PocketShooter.Meta.Entities
{
    /// <summary>
    /// Represents a reward that player can get.
    /// </summary>
    public class PlayerReward
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerReward"/> class.
        /// </summary>
        public PlayerReward()
            : this(0, 0, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerReward"/> class.
        /// </summary>
        /// <param name="cash">The cash reward.</param>
        /// <param name="gold">The gold reward.</param>
        public PlayerReward(int cash, int gold)
            : this(cash, gold, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerReward"/> class.
        /// </summary>
        /// <param name="cash">The cash reward.</param>
        /// <param name="gold">The gold reward.</param>
        /// <param name="experience">The experience resource.</param>
        public PlayerReward(int cash, int gold, int experience)
        {
            this.Cash = cash;
            this.Gold = gold;
            this.Experience = experience;
        }

        /// <summary>
        /// Gets a cash reward.
        /// </summary>
        public int Cash
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a gold reward.
        /// </summary>
        public int Gold
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets an experience reward.
        /// </summary>
        public int Experience
        {
            get;
            private set;
        }

        /// <summary>
        /// Adds a reward to this one.
        /// </summary>
        /// <param name="reward">The reward to add.</param>
        public void Add(PlayerReward reward)
        {
            Cash += reward.Cash;
            Gold += reward.Gold;
            Experience += reward.Experience;
        }
    }
}
