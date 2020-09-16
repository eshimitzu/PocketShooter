namespace Heyworks.PocketShooter.Meta.Entities
{
    /// <summary>
    /// Represents a price that can be configured in different points.
    /// </summary>
    public class Price
    {
        private Price()
        {
        }

        /// <summary>
        /// Gets a zero price.
        /// </summary>
        public static Price Zero { get; } = CreateCash(0);

        /// <summary>
        /// Create a price in cash.
        /// </summary>
        /// <param name="cashAmount">The cash amount.</param>
        public static Price CreateCash(int cashAmount)
        {
            return new Price()
            {
                Type = PriceType.Cash,
                CashAmount = cashAmount,
            };
        }

        /// <summary>
        /// Create a price in cash.
        /// </summary>
        /// <param name="cashAmount">The cash amount.</param>
        public static Price CreateGold(int goldAmount)
        {
            return new Price()
            {
                Type = PriceType.Gold,
                GoldAmount = goldAmount,
            };
        }

        /// <summary>
        /// Create a price in real-world currency.
        /// </summary>
        /// <param name="purchaseId">The in-app purchase id.</param>
        public static Price CreateRealCurrency(string purchaseId)
        {
            return new Price()
            {
                Type = PriceType.RealCurrency,
                PurchaseId = purchaseId,
            };
        }

        /// <summary>
        /// Gets a price type.
        /// </summary>
        public PriceType Type
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a cash amount when <see cref="Type"/> is <see cref="PriceType.Cash"/>.
        /// </summary>
        public int CashAmount
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a gold amount when <see cref="Type"/> is <see cref="PriceType.Gold"/>.
        /// </summary>
        public int GoldAmount
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a purchase id when <see cref="Type"/> is <see cref="PriceType.RealCurrency"/>.
        /// </summary>
        public string PurchaseId
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether this is a zero price.
        /// </summary>
        public bool IsZero
        {
            get => CashAmount == 0 && GoldAmount == 0 && PurchaseId == null;
        }

        public int TicketsAmount { get; set; }
    }
}
