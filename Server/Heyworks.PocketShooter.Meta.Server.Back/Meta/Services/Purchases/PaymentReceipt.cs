using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Entities;

namespace Heyworks.PocketShooter.Meta.Services.Purchases
{
    /// <summary>
    /// Represents a payment receipt.
    /// </summary>
    public abstract class PaymentReceipt
    {
        /// <summary>
        /// Verifies the current receipt and returns the payment transaction if receipt is valid.
        /// Throws an exception if receipt is invalid.
        /// </summary>
        /// <exception cref="InvalidPaymentReceiptException">When the payment receipt is invalid.</exception>
        /// <exception cref="TransactionExistsException">When the payment transaction is already exists in the system.</exception>
        public abstract Task<PaymentTransaction> VerifyAsync();
    }
}
