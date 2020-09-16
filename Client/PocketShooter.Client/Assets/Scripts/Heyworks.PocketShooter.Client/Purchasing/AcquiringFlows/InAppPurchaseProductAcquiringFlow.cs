using Heyworks.PocketShooter.Meta.Communication;
using Heyworks.PocketShooter.Purchasing.Products;
using Heyworks.PocketShooter.Purchasing.PurchaseManager.CommonEntities;
using Microsoft.Extensions.Logging;

namespace Heyworks.PocketShooter.Purchasing.AcquiringFlows
{
    /// <summary>
    /// Represents an object which encapsulates the flow of acquiring of an in-app purchase.
    /// </summary>
    public sealed class InAppPurchaseProductAcquiringFlow : ShopProductAcquiringFlow
    {
        private static bool isPurchaseInProgress;

        private readonly IPurchaseManager purchaseManager;
        private readonly IGameHubClient shopComponent;
        private readonly string purchaseId;

        /// <summary>
        /// Initializes a new instance of the <see cref="InAppPurchaseProductAcquiringFlow"/> class.
        /// </summary>
        /// <param name="purchaseManager"> Object, providing interface to make in-app purchase. </param>
        /// <param name="shopComponent"> Object providing access to the server operations connected with shop. </param>
        /// <param name="purchaseId"> Id of the in-app purchase which acquiring flow is to be encapsulated. </param>
        public InAppPurchaseProductAcquiringFlow(
            IPurchaseManager purchaseManager,
            IGameHubClient shopComponent,
            string purchaseId)
        {
            this.purchaseManager = purchaseManager;
            this.shopComponent = shopComponent;
            this.purchaseId = purchaseId;
        }

        /// <summary>
        /// Starts the flow of product acquiring.
        /// </summary>
        public override void StartAcquiring()
        {
            PurchaseLog.Log.LogTrace("Make purchase attempt. Id: {0}", purchaseId);

            if (!isPurchaseInProgress)
            {
                MakePurchase(purchaseId);
            }
            else
            {
                PurchaseLog.Log.LogWarning("Don't send purchase request to Apple's server because another purchase is in progress now.");
                OnAcquireFailed();
            }
        }

        private void MakePurchase(string purchaseId)
        {
            isPurchaseInProgress = true;

            AddPurchaseManagerEventHandlers();
            purchaseManager.StartPurchaseTransaction(purchaseId);
        }

        private void AddPurchaseManagerEventHandlers()
        {
            purchaseManager.PurchaseTransactionFailed += PurchaseManager_PurchaseTransactionFailed;
            purchaseManager.PurchaseTransactionCanceled += PurchaseManager_PurchaseTransactionCanceled;
            purchaseManager.PurchaseTransactionStarted += PurchaseManager_PurchaseTransactionStarted;
            purchaseManager.TransactionsQueueIncreased += PurchaseManager_TransactionsQueueIncreased;
        }

        private void RemovePurchaseManagerEventHandlers()
        {
            purchaseManager.PurchaseTransactionFailed -= PurchaseManager_PurchaseTransactionFailed;
            purchaseManager.PurchaseTransactionCanceled -= PurchaseManager_PurchaseTransactionCanceled;
            purchaseManager.PurchaseTransactionStarted -= PurchaseManager_PurchaseTransactionStarted;
            purchaseManager.TransactionsQueueIncreased -= PurchaseManager_TransactionsQueueIncreased;
        }

        private void PurchaseManager_PurchaseTransactionStarted(PurchaseTransactionStartedEventArgs e)
        {
            PurchaseLog.Log.LogTrace("Shop. Transaction has been accepted by player.");

            RemovePurchaseManagerEventHandlers();

            var purhcaseConfirmationFlow = new PurchaseConfirmationFlow(purchaseManager, shopComponent);
            purhcaseConfirmationFlow.ConfirmationFailed += PurhcaseConfirmationFlow_ConfirmationFailed;
            purhcaseConfirmationFlow.ConfirmationSucceeded += PurhcaseConfirmationFlow_ConfirmationSucceeded;
            purhcaseConfirmationFlow.ConfirmPurchase(e.Receipt);
        }

        private void PurchaseManager_TransactionsQueueIncreased(TransactionType transactionType)
        {
            PurchaseLog.Log.LogTrace("Shop. New acquire. Transactions queue has been increased. TransactionType: {0}", transactionType);

            purchaseManager.StartPendingPurchaseTransaction();
        }

        private void PurchaseManager_PurchaseTransactionFailed(string message)
        {
            isPurchaseInProgress = false;

            PurchaseLog.Log.LogWarning("Shop. Purchase has FAILED. Message: {0};", message);

            RemovePurchaseManagerEventHandlers();
            OnAcquireFailed();
        }

        private void PurchaseManager_PurchaseTransactionCanceled(string message)
        {
            isPurchaseInProgress = false;

            PurchaseLog.Log.LogWarning("Shop. Purchase has been CANCELED. Message: {0};", message);

            RemovePurchaseManagerEventHandlers();
            OnAcquireFailed();
        }

        private void PurhcaseConfirmationFlow_ConfirmationSucceeded()
        {
            isPurchaseInProgress = false;
            OnAcquireSucceeded();
        }

        private void PurhcaseConfirmationFlow_ConfirmationFailed()
        {
            isPurchaseInProgress = false;
            OnAcquireFailed();
        }
    }
}
