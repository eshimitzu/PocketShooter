using System;
using System.Collections.Generic;
using Heyworks.PocketShooter.Core.SchedulerManager;
using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.Purchasing.PurchaseManager.CommonEntities;
using Microsoft.Extensions.Logging;

namespace Heyworks.PocketShooter.Purchasing.PurchaseManager.Emulator
{
    /// <summary>
    /// Represents purchase logic on iOS operation system.
    /// </summary>
    public sealed class EmulatorPurchaseManager : IPurchaseManager
    {
        #region [Private fields]

        private const float DELAY_BEFORE_PURCHASES_RECEIVED = 2;
        private bool purchasesReceived;
        private IReadOnlyList<InAppPurchase> purchases;

        #endregion

        #region [Events]
#pragma warning disable 67
        /// <summary>
        /// Event fires if the billing is not supported.
        /// </summary>
        public event Action<string> BillingIsNotSupported;

        /// <summary>
        /// Event fires when all in-app purchases has been successfully received.
        /// </summary>
        public event Action PurchasesLoaded;

        /// <summary>
        /// Occurs when transactions queue has been increased.
        /// </summary>
        public event Action<TransactionType> TransactionsQueueIncreased;

        /// <summary>
        /// Event fires if the list of purchases failed to be loaded.
        /// </summary>
        public event Action<string> PurchasesFailedToBeLoaded;

        /// <summary>
        /// Event fires when the purchase transaction is successfully opened.
        /// </summary>
        public event Action<PurchaseTransactionStartedEventArgs> PurchaseTransactionStarted;

        /// <summary>
        /// Event fires when the purchase transaction is restored.
        /// </summary>
        public event Action<PurchaseTransactionStartedEventArgs> RestoreTransactionStarted;

        /// <summary>
        /// Fired if the payment or transaction opening haven't been performed successfully.
        /// </summary>
        public event Action<string> PurchaseTransactionFailed;

        /// <summary>
        /// Fired if the purchase have been canceled by user.
        /// </summary>
        public event Action<string> PurchaseTransactionCanceled;

        /// <summary>
        /// Event fires when the purchase transaction is closed on the e-market.
        /// </summary>
        public event Action PurchaseTransactionClosed;

        /// <summary>
        /// Fired when all transactions from the user's purchase history have successfully been added back to the queue.
        /// </summary>
        public event Action RestoreTransactionsFinished;

        /// <summary>
        /// Fired when an error is encountered while adding transactions from the user's purchase history back to the queue
        /// </summary>
        public event Action<string> RestoreTransactionsFailed;
#pragma warning restore 67
        #endregion

        #region [Properties]

        /// <summary>
        /// Gets a value indicating whether some transaction is being handled at the moment.
        /// </summary>
        public bool IsTransactionInProgress
        {
            get; private set;
        }

        /// <summary>
        /// Gets a value indicating whether there exist at least one not finished purchase transaction.
        /// </summary>
        public bool HasPendingTransactions
        {
            get
            {
                return false;
            }
        }

        #endregion

        public EmulatorPurchaseManager(IReadOnlyList<InAppPurchase> purchases)
        {
            this.purchases = purchases;
        }

        #region [Public methods]

        /// <summary>
        /// Send request to the e-market server to load all in-apps.
        /// </summary>
        /// <param name="purchasesIds"> List of all purchases' identifiers to get from e-market server. </param>
        public void LoadPurchases(string[] purchasesIds)
        {
            PurchaseLog.Log.LogTrace("Emulating loading purchase list.");

            SchedulerManager.Instance.CallActionWithDelay(null, () => LoadPurchaseCoroutine(), DELAY_BEFORE_PURCHASES_RECEIVED);
        }

        /// <summary>
        /// Initializes the purchase manager.
        /// </summary>
        public void Initialize(IReadOnlyList<InAppPurchase> purchases)
        {
            LoadPurchases(null);
        }

        /// <summary>
        /// Gets the list of objects, each describing a single in-app purchase.
        /// </summary>
        public IList<PurchaseRepresentationData> GetPurchases()
        {
            PurchaseLog.Log.LogTrace("Emulating getting purchase data.");

            if (!purchasesReceived)
            {
                return new List<PurchaseRepresentationData>();
            }

            var unsortedList = new List<PurchaseRepresentationData>();
            foreach (var inAppPurchase in purchases)
            {
                var id = inAppPurchase.Id;
                var description = GetInAppPurchaseDescription(inAppPurchase);
                string title = id;
                var purchaseRepresentationData = new PurchaseRepresentationData(id, title, description, "$", (decimal)inAppPurchase.PriceUSD, "TestingTransactionID");
                unsortedList.Add(purchaseRepresentationData);
            }

            return unsortedList;
        }

        /// <summary>
        /// Start the transaction, which not consumed yet and is waiting for being consumed (if there exists one).
        /// </summary>
        public void StartPendingPurchaseTransaction()
        {
            PurchaseLog.Log.LogTrace("Do nothing at attempt to finish all pending transactions in emulator.");
        }

        /// <summary>
        /// Start transaction for purchase with specified Id.
        /// </summary>
        /// <param name="purchaseId"> Id of an item to buy. </param>
        public void StartPurchaseTransaction(string purchaseId)
        {
            PurchaseLog.Log.LogTrace("Emulating making purchase.");

            IsTransactionInProgress = true;
            var args = new PurchaseTransactionStartedEventArgs(purchaseId, purchaseId);
            OnPurchaseTransactionStarted(args);
        }

        /// <summary>
        /// Send request to the e-market server to close current purchase transaction (if there exists one).
        /// </summary>
        public void CloseCurrentPurchaseTransaction()
        {
            PurchaseLog.Log.LogTrace("Emulating finishing current transaction.");

            IsTransactionInProgress = false;
            OnPurchaseTransactionClosed();
        }

        /// <summary>
        /// Sets aside the purchase transaction, which is currently is waiting for being consumed (if there exists one).
        /// </summary>
        /// <remarks> It does NOT consumes the purchase but leaves it as the pending one (i.e. transaction will show up the next app launch). </remarks>
        public void PostponeCurrentPurchaseTransaction()
        {
        }

        /// <summary>
        /// Restores all previous transactions.  This is used when a user gets a new device and they need to restore their old purchases.
        /// DO NOT call this on every launch.  It will prompt the user for their password. Each transaction that is restored will have the normal
        /// purchaseSuccessfulEvent fire for when restoration is complete.
        /// </summary>
        public void RestoreTransactions()
        {
            var handlers = RestoreTransactionsFinished;
            if (handlers != null)
            {
                handlers();
            }
        }

        #endregion

        #region [Private methods]

        private void LoadPurchaseCoroutine()
        {
            purchasesReceived = true;
            OnPurchasesLoaded();
        }

        private string GetInAppPurchaseDescription(InAppPurchase inAppPurchase)
        {
            return string.Empty;
        }

        #endregion

        #region [Event invokators]

        private void OnPurchasesLoaded()
        {
            PurchaseLog.Log.LogTrace("Emulating OnPurchasesListIsLoaded.");

            var handler = PurchasesLoaded;
            if (handler != null)
            {
                handler();
            }
        }

        private void OnPurchaseTransactionClosed()
        {
            PurchaseLog.Log.LogTrace("Emulating OnPurchaseSucceded.");

            var handler = PurchaseTransactionClosed;
            if (handler != null)
            {
                handler();
            }
        }

        private void OnPurchaseTransactionStarted(PurchaseTransactionStartedEventArgs e)
        {
            PurchaseLog.Log.LogTrace("Emulating OnTransactionStarted.");

            var handler = PurchaseTransactionStarted;
            if (handler != null)
            {
                handler(e);
            }
        }

        #endregion
    }
}
