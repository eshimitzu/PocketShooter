namespace Heyworks.PocketShooter.Meta.Services.Purchases
{
    /// <summary>
    /// Represents deserialized object containing Google In-App Purchase information.
    /// </summary>
    internal class GoogleReceiptData
    {
        /// <summary>
        /// Gets or sets the unique order identifier for the transaction. This corresponds to the Google Wallet Order ID.
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// Gets or sets the item's product identifier.
        /// Every item has a product ID, which you must specify in the application's product list on the Google Play Developer Console.
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// Gets or sets the token that uniquely identifies a purchase for a given item and user pair.
        /// </summary>
        public string PurchaseToken { get; set; }

        /// <summary>
        /// Gets or sets the purchase state of the order. Possible values are 0 (purchased), 1 (canceled), or 2 (refunded).
        /// </summary>
        public GooglePurchaseState PurchaseState { get; set; }

        /// <summary>
        /// Gets or sets the application package from which the purchase originated.
        /// </summary>
        public string PackageName { get; set; }

        /// <summary>
        /// Gets or sets the date and time of the given purchase.
        /// </summary>
        public long PurchaseTime { get; set; }

        /// <summary>
        /// Gets or sets a developer-specified string that contains supplemental information about an order.
        /// You can specify a value for this field when you make a getBuyIntent request.
        /// </summary>
        public string DeveloperPayload { get; set; }

        /// <summary>
        /// Gets a value indicating whether the transaction performed in sandbox mode.
        /// </summary>
        public bool IsSandbox
        {
            get { return string.IsNullOrEmpty(OrderId); }
        }
    }

    /// <summary>
    /// Google purchase state values.
    /// </summary>
    internal enum GooglePurchaseState
    {
        /// <summary>
        /// Product was purchased
        /// </summary>
        Purchased = 0,

        /// <summary>
        /// Product purchase was canceled.
        /// </summary>
        Canceled = 1,

        /// <summary>
        /// Purchase was refunded.
        /// </summary>
        Refunded = 2,
    }

    /// <summary>
    /// Google purchase state values.
    /// </summary>
    internal enum AndroidPurchaseConsumptionState
    {
        /// <summary>
        /// Product is not consumed yet.
        /// </summary>
        YetToBeConsumed = 0,

        /// <summary>
        /// Product is consumed.
        /// </summary>
        Consumed = 1,
    }
}
