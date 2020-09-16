using Orleans;
using System.Threading.Tasks;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public interface IShopGrain : IGrainWithGuidKey
    {
        /// <summary>
        /// Makes a purchase.
        /// </summary>
        /// <param name="receiptData">The purchase receipt data.</param>
        /// <exception cref="Services.Purchases.InvalidPaymentReceiptException">When the payment receipt is invalid.</exception>
        /// <exception cref="Services.TransactionExistsException">When the payment transaction is already exists in the system.</exception>
        Task MakePurchase(string receiptData);

        /// <summary>
        /// Buys a roster product with specified id. Returns <c>true</c> if succeeds; otherwise returns <c>false</c>.
        /// </summary>
        /// <param name="productId">The roster product id.</param>
        /// <returns><c>true</c> if succeeds; otherwise returns <c>false</c>.</returns>
        Task<bool> BuyRosterProduct(string productId);

        /// <summary>
        /// Buys a shop product with specified id. Returns <c>true</c> if succeeds; otherwise returns <c>false</c>.
        /// </summary>
        /// <param name="productId">The shop product id.</param>
        /// <returns><c>true</c> if succeeds; otherwise returns <c>false</c>.</returns>
        Task<bool> BuyShopProduct(string productId);
    }
}
