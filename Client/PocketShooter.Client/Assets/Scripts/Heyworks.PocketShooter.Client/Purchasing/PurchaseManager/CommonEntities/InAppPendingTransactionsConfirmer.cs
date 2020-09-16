using System;
using Heyworks.PocketShooter.Meta.Communication;
using Heyworks.PocketShooter.Purchasing.Products;
using Microsoft.Extensions.Logging;

namespace Heyworks.PocketShooter.Purchasing.PurchaseManager.CommonEntities
{
    /// <summary>
    /// Represents an object which responsible for confirm pending in-app transaction.
    /// </summary>
    public class InAppPendingTransactionsConfirmer
    {
        private readonly IPurchaseManager purchaseManager;
        private readonly IGameHubClient shopComponent;
        private bool isConfirmationInProgress;

        /// <summary>
        /// Event fires when some product acquiring has just been finished either successfully or not.
        /// </summary>
        public event Action ProductAcquiringFinished;

        /// <summary>
        /// Initializes a new instance of the <see cref="InAppPendingTransactionsConfirmer"/> class.
        /// </summary>
        /// <param name="purchaseManager">The purchase manager.</param>
        public InAppPendingTransactionsConfirmer(IPurchaseManager purchaseManager, IGameHubClient shopComponent)
        {
            this.purchaseManager = purchaseManager;
            this.shopComponent = shopComponent;
            purchaseManager.TransactionsQueueIncreased += PurchaseManager_TransactionsQueueIncreased;
        }

        /// <summary>
        /// Finishes the pending purchases.
        /// </summary>
        public void FinishPendingPurchases()
        {
            var canFinishPendingTransactions = purchaseManager.HasPendingTransactions && !isConfirmationInProgress;
            if (canFinishPendingTransactions)
            {
                purchaseManager.PurchaseTransactionStarted += PurchaseManager_PendingPurchaseTransactionStarted;
                purchaseManager.StartPendingPurchaseTransaction();
            }
        }

        private void PurchaseManager_PendingPurchaseTransactionStarted(PurchaseTransactionStartedEventArgs e)
        {
            PurchaseLog.Log.LogTrace("Shop. Transactions confirmer. Pending transaction started.");

            purchaseManager.PurchaseTransactionStarted -= PurchaseManager_PendingPurchaseTransactionStarted;

            var purhcaseConfirmationFlow = new PurchaseConfirmationFlow(purchaseManager, shopComponent);
            purhcaseConfirmationFlow.ConfirmationSucceeded += PurhcaseConfirmationFlow_ConfirmationFinished;
            purhcaseConfirmationFlow.ConfirmationFailed += PurhcaseConfirmationFlow_ConfirmationFinished;
            isConfirmationInProgress = true;
            purhcaseConfirmationFlow.ConfirmPurchase(e.Receipt);
        }

        private void PurchaseManager_TransactionsQueueIncreased(TransactionType transactionType)
        {
            PurchaseLog.Log.LogTrace("Shop.Confirm pending transaction. Transactions queue has been increased. TransactionType: {0}", transactionType);

            if (transactionType == TransactionType.Confirm)
            {
                FinishPendingPurchases();
            }
        }

        private void PurhcaseConfirmationFlow_ConfirmationFinished()
        {
            isConfirmationInProgress = false;
            OnProductAcquiringFinished();
            FinishPendingPurchases();
        }

        private void OnProductAcquiringFinished()
        {
            var handler = ProductAcquiringFinished;
            if (handler != null)
            {
                handler();
            }
        }
    }
}
