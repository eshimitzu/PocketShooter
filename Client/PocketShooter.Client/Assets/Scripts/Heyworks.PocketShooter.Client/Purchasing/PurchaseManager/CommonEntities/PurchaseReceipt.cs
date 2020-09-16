namespace Heyworks.PocketShooter.Purchasing.PurchaseManager.CommonEntities
{
    /// <summary>
    /// Encapsulates receipt for the concrete in-app purchase.
    /// </summary>
    public class PurchaseReceipt
    {
        /// <summary>
        /// Gets an ID of the transaction, receipt for which is represented by the object.
        /// </summary>
        public string TransactionId { get; private set; }

        /// <summary>
        /// Gets billing information about started transaction.
        /// </summary>
        public string ReceiptData { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PurchaseReceipt"/> class.
        /// </summary>
        /// <param name="id"> ID of the purchase, receipt for which is represented by the object. </param>
        /// <param name="receiptData"> Billing information about started transaction. </param>
        public PurchaseReceipt(string id, string receiptData)
        {
            TransactionId = id;
            ReceiptData = receiptData;
        }
    }
}
