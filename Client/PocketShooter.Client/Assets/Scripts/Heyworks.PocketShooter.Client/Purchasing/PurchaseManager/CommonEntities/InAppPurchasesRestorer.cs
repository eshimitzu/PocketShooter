using System;
using Heyworks.PocketShooter.Meta.Communication;
using Heyworks.PocketShooter.Purchasing.Products;
using Microsoft.Extensions.Logging;

namespace Heyworks.PocketShooter.Purchasing.PurchaseManager.CommonEntities
{
    /// <summary>
    /// Represents an object which responsible for restoring non consumable in-app purchases.
    /// </summary>
    public class InAppPurchasesRestorer
    {
        #region [Private fields]

        private readonly IPurchaseManager purchaseManager;
        private readonly IGameHubClient shopComponent;
        private bool isConfirmationInProgress;

        #endregion

        #region [Events]

        /// <summary>
        /// Fired when an error is encountered while adding transactions from the user's purchase history back to the queue.
        /// </summary>
        public event Action RestoreTransactionsFailed;

        /// <summary>
        /// Fired when all transactions from the user's purchase history have successfully been added back to the queue.
        /// </summary>
        public event Action RestoreTransactionsFinished;

        /// <summary>
        /// Occurs when restore transactions has been started.
        /// </summary>
        public event Action RestoreTransactionsStarted;

        /// <summary>
        /// Event fires when some product acquiring has just been finished either successfully or not.
        /// </summary>
        public event Action ProductAcquiringFinished;
    
        #endregion

        #region [Construction and initialization]

        /// <summary>
        /// Initializes a new instance of the <see cref="InAppPurchasesRestorer" /> class.
        /// </summary>
        /// <param name="purchaseManager">The purchase manager.</param>
        /// <param name="shopComponent">The shop component.</param>
        public InAppPurchasesRestorer(IPurchaseManager purchaseManager, IGameHubClient shopComponent)
        {
            this.purchaseManager = purchaseManager;
            this.shopComponent = shopComponent;
        }

        #endregion

        #region [Public methods]

        /// <summary>
        /// Restores all non-consumable in-app purchases.
        /// </summary>
        public void RestorePurchases()
        {
            AddEventHandlers();
            OnRestoreTransactionsStarted();
            purchaseManager.RestoreTransactions();
        }

        #endregion

        #region [Event handlers]

        private void PurchaseManager_PendingPurchaseTransactionStarted(PurchaseTransactionStartedEventArgs e)
        {
            PurchaseLog.Log.LogTrace("Shop. Restore. Pending transaction started.");

            purchaseManager.PurchaseTransactionStarted -= PurchaseManager_PendingPurchaseTransactionStarted;

            var purhcaseConfirmationFlow = new PurchaseConfirmationFlow(purchaseManager, shopComponent);
            purhcaseConfirmationFlow.ConfirmationSucceeded += PurhcaseConfirmationFlow_ConfirmationFinished;
            purhcaseConfirmationFlow.ConfirmationFailed += PurhcaseConfirmationFlow_ConfirmationFinished;
            isConfirmationInProgress = true;
            purhcaseConfirmationFlow.ConfirmPurchase(e.Receipt);
        }

        private void PurchaseManager_RestoreTransactionsFailed(string errorMessage)
        {
            RemoveEventHandlers();
            OnRestoreTransactionsFailed();
        }

        private void PurchaseManager_RestoreTransactionsFinished()
        {
            RemoveEventHandlers();
            OnRestoreTransactionsFinished();
        }

        private void PurchaseManager_TransactionsQueueIncreased(TransactionType transactionType)
        {
            PurchaseLog.Log.LogTrace("Shop. Restore. Transactions queue has been increased. TransactionType: {0}", transactionType);

            if (transactionType == TransactionType.Restore)
            {
                TryFinishPendingPurchases();
            }
        }

        private void PurhcaseConfirmationFlow_ConfirmationFinished()
        {
            isConfirmationInProgress = false;
            OnProductAcquiringFinished();
            TryFinishPendingPurchases();
        }

        #endregion

        #region [Private methods]

        private void AddEventHandlers()
        {
            purchaseManager.RestoreTransactionsFinished += PurchaseManager_RestoreTransactionsFinished;
            purchaseManager.RestoreTransactionsFailed += PurchaseManager_RestoreTransactionsFailed;
            purchaseManager.TransactionsQueueIncreased += PurchaseManager_TransactionsQueueIncreased;
        }

        private void OnRestoreTransactionsFailed()
        {
            var handler = RestoreTransactionsFailed;
            if (handler != null)
            {
                handler();
            }
        }

        private void OnRestoreTransactionsFinished()
        {
            var handler = RestoreTransactionsFinished;
            if (handler != null)
            {
                handler();
            }
        }
    
        private void OnProductAcquiringFinished()
        {
            var handler = ProductAcquiringFinished;
            if (handler != null)
            {
                handler();
            }
        }

        private void OnRestoreTransactionsStarted()
        {
            var handler = RestoreTransactionsStarted;
            if (handler != null) handler();
        }

        private void RemoveEventHandlers()
        {
            purchaseManager.RestoreTransactionsFinished -= PurchaseManager_RestoreTransactionsFinished;
            purchaseManager.RestoreTransactionsFailed -= PurchaseManager_RestoreTransactionsFailed;
            purchaseManager.TransactionsQueueIncreased -= PurchaseManager_TransactionsQueueIncreased;
        }

        private void TryFinishPendingPurchases()
        {
            var canFinishPendingTransactions = purchaseManager.HasPendingTransactions && !isConfirmationInProgress;
            if (canFinishPendingTransactions)
            {
                purchaseManager.PurchaseTransactionStarted += PurchaseManager_PendingPurchaseTransactionStarted;
                purchaseManager.StartPendingPurchaseTransaction();
            }
        }

        #endregion
    }
}