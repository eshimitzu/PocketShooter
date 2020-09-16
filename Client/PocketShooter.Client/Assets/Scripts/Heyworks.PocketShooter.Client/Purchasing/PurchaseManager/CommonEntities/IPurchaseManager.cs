using System;
using System.Collections.Generic;
using Heyworks.PocketShooter.Meta.Entities;

namespace Heyworks.PocketShooter.Purchasing.PurchaseManager.CommonEntities
{
    /// <summary>
    /// Provides functionality to make in-app purchases.
    /// </summary>
    public interface IPurchaseManager
    {
        /// <summary>
        /// Gets a value indicating whether there exist at least one not finished purchase transaction.
        /// </summary>
        bool HasPendingTransactions { get; }

        /// <summary>
        /// Event fires if the billing is not supported.
        /// </summary>
        event Action<string> BillingIsNotSupported;

        /// <summary>
        /// Event fires when all in-app purchases has been successfully received.
        /// </summary>
        event Action PurchasesLoaded;

        /// <summary>
        /// Occurs when transactions queue has been increased.
        /// </summary>
        event Action<TransactionType> TransactionsQueueIncreased;

        /// <summary>
        /// Event fires if the list of purchases failed to be loaded.
        /// </summary>
        event Action<string> PurchasesFailedToBeLoaded;

        /// <summary>
        /// Event fires when the purchase transaction is successfully opened.
        /// </summary>
        event Action<PurchaseTransactionStartedEventArgs> PurchaseTransactionStarted;
    
        /// <summary>
        /// Fired if the payment or transaction opening haven't been performed successfully.
        /// </summary>
        event Action<string> PurchaseTransactionFailed;

        /// <summary>
        /// Fired if the purchase have been canceled by user.
        /// </summary>
        event Action<string> PurchaseTransactionCanceled;

        /// <summary>
        /// Event fires when the purchase transaction is closed on the e-market.
        /// </summary>
        event Action PurchaseTransactionClosed;

        /// <summary>
        /// Fired when all transactions from the user's purchase history have successfully been added back to the queue.
        /// </summary>
        event Action RestoreTransactionsFinished;

        /// <summary>
        /// Fired when an error is encountered while adding transactions from the user's purchase history back to the queue.
        /// </summary>
        event Action<string> RestoreTransactionsFailed;

        /// <summary>
        /// Initializes the purchase manager.
        /// </summary>
        void Initialize(IReadOnlyList<InAppPurchase> purchases);

        /// <summary>
        /// Gets the list of objects, each describing a single in-app purchase.
        /// </summary>
        IList<PurchaseRepresentationData> GetPurchases();

        /// <summary>
        /// Start the transaction, which not consumed yet and is waiting for being consumed (if there exists one).
        /// </summary>
        void StartPendingPurchaseTransaction();

        /// <summary>
        /// Start transaction for purchase with specified Id.
        /// </summary>
        /// <param name="purchaseId"> Id of an item to buy. </param>
        void StartPurchaseTransaction(string purchaseId);

        /// <summary>
        /// Send request to the e-market server to close current purchase transaction (if there exists one).
        /// </summary>
        void CloseCurrentPurchaseTransaction();

        /// <summary>
        /// Sets aside the purchase transaction, which is currently is waiting for being consumed (if there exists one).
        /// </summary>
        /// <remarks> It does NOT consumes the purchase but leaves it as the pending one (i.e. transaction will show up the next app launch). </remarks>
        void PostponeCurrentPurchaseTransaction();

        /// <summary>
        /// Restores all previous transactions.  This is used when a user gets a new device and they need to restore their old purchases.
        /// DO NOT call this on every launch.  It will prompt the user for their password. Each transaction that is restored will have the normal
        /// purchaseSuccessfulEvent fire for when restoration is complete.
        /// </summary>
        void RestoreTransactions();
    }
}