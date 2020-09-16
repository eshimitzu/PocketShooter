using System;

namespace Heyworks.PocketShooter.Meta.Entities
{
    /// <summary>
    /// Represents a payment transaction.
    /// </summary>
    public class PaymentTransaction
    {
        /// <summary>
        /// Gets or sets the unique entity id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the player id.
        /// </summary>
        public Guid PlayerId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the product id.
        /// </summary>
        public string ProductId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the transaction id.
        /// </summary>
        public string TransactionId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the purchase date.
        /// </summary>
        public DateTime PurchaseDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether the transaction performed in sandbox mode.
        /// </summary>
        public virtual bool IsSandbox
        {
            get
            {
                return false;
            }
        }
    }
}
