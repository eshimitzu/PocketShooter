namespace Heyworks.PocketShooter.Meta.Entities
{
    /// <summary>
    /// Represents the Google payment transaction.
    /// </summary>
    public class GooglePaymentTransaction : PaymentTransaction
    {
        /// <summary>
        /// Gets or sets the application package from which the purchase originated..
        /// </summary>
        public string PackageName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the token that uniquely identifies a purchase for a given item and user pair.
        /// </summary>
        public string PurchaseToken
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a developer-specified string that contains supplemental information about an order.
        /// You can specify a value for this field when you make a getBuyIntent request.
        /// </summary>
        public string DeveloperPayload
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether the transaction performed in sandbox mode.
        /// </summary>
        public override bool IsSandbox
        {
            get
            {
                return string.IsNullOrEmpty(TransactionId);
            }
        }
    }
}
