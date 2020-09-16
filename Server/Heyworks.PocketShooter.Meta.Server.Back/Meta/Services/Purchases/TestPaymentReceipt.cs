using System;
using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Entities;

namespace Heyworks.PocketShooter.Meta.Services.Purchases
{
    /// <summary>
    /// Represents payment receipt for the tests only.
    /// </summary>
    internal class TestPaymentReceipt : PaymentReceipt
    {
        private readonly string productId;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestPaymentReceipt"/> class.
        /// </summary>
        /// <param name="productId">The product id.</param>
        public TestPaymentReceipt(string productId)
        {
            this.productId = productId;
        }

        #region [Base class Overrides]

        /// <summary>
        /// Verifies the current receipt and returns the payment transaction.
        /// </summary>
        public override Task<PaymentTransaction> VerifyAsync()
        {
            return Task.FromResult(new PaymentTransaction()
            {
                ProductId = productId,
                TransactionId = Guid.NewGuid().ToString(),
                PurchaseDate = DateTime.UtcNow,
            });
        }

        public override string ToString()
        {
            return string.Format($@"""Type: ""{nameof(TestPaymentReceipt)}"", Data: ""{productId}""""");
        }

        #endregion [Base class Overrides]
    }
}
