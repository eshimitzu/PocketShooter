using Heyworks.Realtime.Serialization;

namespace Heyworks.PocketShooter.Realtime.Data
{
    /// <summary>
    /// Describes applied heal.
    /// </summary>
    public readonly struct HealInfo
    {
        /// <summary>
        /// Type of the heal.
        /// </summary>
        public readonly HealType Type;

        /// <summary>
        /// Healed health amount.
        /// </summary>
        //[Limit(0.0f, 1000f, 0.001f)]
        public readonly float Amount;

        /// <summary>
        /// Initializes a new instance of the <see cref="HealInfo"/> struct.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="amount">Amount.</param>
        public HealInfo(HealType type, float amount)
        {
            Amount = amount;
            Type = type;
        }
    }
}