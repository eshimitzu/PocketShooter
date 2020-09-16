namespace Heyworks.PocketShooter.Meta.Entities
{
    public class ItemStats
    {
        private ItemStats()
        {
        }

        public ItemStats(int power, int attack, int health, int armor, double movement, int distance, int capacity, double reload)
        {
            Power = power;
            Attack = attack;
            Health = health;
            Armor = armor;
            Movement = movement;
            Distance = distance;
            Capacity = capacity;
            Reload = reload;
        }

        public static ItemStats CreateForTrooper(int power, int attack, int health, int armor, double movement) =>
            new ItemStats(power, attack, health, armor, movement, 0, 0, 0.0);

        public static ItemStats CreateForWeapon(int power, int attack, int distance, int capacity, double reload) =>
            new ItemStats(power, attack, 0, 0, 0, distance, capacity, reload);

        public static ItemStats CreateForHelmet(int power, int health) =>
            new ItemStats(power, 0, health, 0, 0.0, 0, 0, 0.0);

        public static ItemStats CreateForArmor(int power, int armor) =>
            new ItemStats(power, 0, 0, armor, 0.0, 0, 0, 0.0);

        public static ItemStats CreateEmpty() => new ItemStats();

        /// <summary>
        /// Gets the power stat.
        /// </summary>
        public int Power { get; private set; }

        /// <summary>
        /// Gets the attack stat.
        /// </summary>
        public int Attack { get; private set; }

        /// <summary>
        /// Gets the health stat.
        /// </summary>
        public int Health { get; private set; }

        /// <summary>
        /// Gets the armor stat.
        /// </summary>
        public int Armor { get; private set; }

        /// <summary>
        /// Gets the movement stat.
        /// </summary>
        public double Movement { get; private set; }

        /// <summary>
        /// Gets the distance stat.
        /// </summary>
        public int Distance { get; private set; }

        /// <summary>
        /// Gets the capacity stat.
        /// </summary>
        public int Capacity { get; private set; }

        /// <summary>
        /// Gets the reload stat.
        /// </summary>
        public double Reload { get; private set; }

        /// <summary>
        /// Gets the sum of two item stats.
        /// </summary>
        /// <param name="first">The first item stats.</param>
        /// <param name="second">The second item stats.</param>
        public static ItemStats Sum(ItemStats first, ItemStats second)
        {
            return new ItemStats(
                first.Power + second.Power,
                first.Attack + second.Attack,
                first.Health + second.Health,
                first.Armor + second.Armor,
                first.Movement + second.Movement,
                first.Distance + second.Distance,
                first.Capacity + second.Capacity,
                first.Reload + second.Reload);
        }
    }
}
