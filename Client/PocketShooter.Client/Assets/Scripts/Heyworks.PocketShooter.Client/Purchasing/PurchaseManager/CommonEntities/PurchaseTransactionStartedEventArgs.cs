namespace Heyworks.PocketShooter.Purchasing.PurchaseManager.CommonEntities
{
    /// <summary>
    /// Encapsulates data to be received, when purchase transaction is started.
    /// </summary>
    public class PurchaseTransactionStartedEventArgs
    {
        /// <summary>
        /// Gets the receipt for in-app purchase.
        /// </summary>
        public PurchaseReceipt Receipt { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PurchaseTransactionStartedEventArgs"/> class.
        /// </summary>
        /// <param name="purchaseId"> ID of the purchase, receipt for which is represented by the object. </param>
        /// <param name="receiptData"> Billing information about started transaction. </param>
        public PurchaseTransactionStartedEventArgs(string purchaseId, string receiptData)
        {
            Receipt = new PurchaseReceipt(purchaseId, receiptData);
        }
    }
}
