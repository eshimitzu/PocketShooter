namespace Heyworks.PocketShooter.Meta.Entities
{
    /// <summary>
    /// Represents an in-app purchase entity.
    /// </summary>
    public class InAppPurchase
    {
        /// <summary>
        /// Gets or sets the purchase id.
        /// </summary>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an in-app purchase type.
        /// </summary>
        public InAppPurchaseType Type
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the real approximate price of the given purchase in US dollars.
        /// </summary>
        public double PriceUSD
        {
            get;
            set;
        }
    }
}
