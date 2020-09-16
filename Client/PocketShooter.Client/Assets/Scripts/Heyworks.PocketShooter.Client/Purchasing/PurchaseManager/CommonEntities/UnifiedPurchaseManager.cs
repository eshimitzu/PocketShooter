using System;
using System.Collections.Generic;
using System.Linq;
using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.Modules.Analytics;
using Microsoft.Extensions.Logging;
using UnityEngine;
using UnityEngine.Purchasing;

// Deriving the Purchaser class from IStoreListener enables it to receive messages from Unity Purchasing.
namespace Heyworks.PocketShooter.Purchasing.PurchaseManager.CommonEntities
{
    public class UnifiedPurchaseManager : IPurchaseManager, IStoreListener
    {
        private IStoreController storeController;
        private IExtensionProvider storeExtensionProvider;
        private readonly Queue<Product> transactionsQueue = new Queue<Product>();
        private List<PurchaseRepresentationData> purchasesInfo = new List<PurchaseRepresentationData>();
        private bool isRestoreInProgress;
        private bool isNewAcqureInProgress;

        /// <summary>
        /// Gets a value indicating whether there exist at least one not finished purchase transaction.
        /// </summary>
        public bool HasPendingTransactions
        {
            get
            {
                return transactionsQueue.Count > 0;
            }
        }

        /// <summary>
        /// Event fires if the billing is not supported.
        /// </summary>
        public event Action<string> BillingIsNotSupported;

        /// <summary>
        /// Occurs when transactions queue has been increased.
        /// </summary>
        public event Action<TransactionType> TransactionsQueueIncreased;

        /// <summary>
        /// Event fires if the list of purchases failed to be loaded.
        /// </summary>
        public event Action<string> PurchasesFailedToBeLoaded;

        /// <summary>
        /// Fired if the payment or transaction opening haven't been performed successfully.
        /// </summary>
        public event Action<string> PurchaseTransactionFailed;

        /// <summary>
        /// Fired if the purchase have been canceled by user.
        /// </summary>
        public event Action<string> PurchaseTransactionCanceled;

        /// <summary>
        /// Event fires when all in-app purchases has been successfully received.
        /// </summary>
        public event Action PurchasesLoaded;

        /// <summary>
        /// Fired when all transactions from the user's purchase history have successfully been added back to the queue.
        /// </summary>
        public event Action RestoreTransactionsFinished;

        /// <summary>
        /// Fired when an error is encountered while adding transactions from the user's purchase history back to the queue.
        /// </summary>
        public event Action<string> RestoreTransactionsFailed;

        /// <summary>
        /// Event fires when the purchase transaction is successfully opened.
        /// </summary>
        public event Action<PurchaseTransactionStartedEventArgs> PurchaseTransactionStarted;

        /// <summary>
        /// Event fires when the purchase transaction is closed on the e-market.
        /// </summary>
        public event Action PurchaseTransactionClosed;

        /// <summary>
        /// Initializes the purchase manager.
        /// </summary>
        public void Initialize(IReadOnlyList<InAppPurchase> products)
        {
            // If we have already connected to Purchasing ...
            if (IsInitialized())
            {
                // ... we are done here.
                return;
            }

            // Create a builder, first passing in a suite of Unity provided stores.
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            foreach (var product in products)
            {
                builder.AddProduct(product.Id, GetProductType(product.Type));
            }

            // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
            // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
            UnityPurchasing.Initialize(this, builder);

            var ids = products.Select(item => item.Id).Aggregate(string.Empty, (current, id) => current + (id + "\n"));

            PurchaseLog.Log.LogTrace("Initializing unified purchase manager with IDs:\n {0}", ids);
        }

        /// <summary>
        /// Gets the list of objects, each describing a single in-app purchase.
        /// </summary>
        public IList<PurchaseRepresentationData> GetPurchases()
        {
            return purchasesInfo;
        }

        /// <summary>
        /// Restores all previous transactions.  This is used when a user gets a new device and they need to restore their old purchases.
        /// DO NOT call this on every launch.  It will prompt the user for their password. Each transaction that is restored will have the normal
        /// purchaseSuccessfulEvent fire for when restoration is complete.
        /// </summary>
        public void RestoreTransactions()
        {
            if (!IsInitialized())
            {
                PurchaseLog.Log.LogError("RestorePurchases FAIL. Not initialized.");

                return;
            }

            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.OSXPlayer)
            {
                PurchaseLog.Log.LogTrace("RestorePurchases started ...");
                isRestoreInProgress = true;
                var apple = storeExtensionProvider.GetExtension<IAppleExtensions>();
                apple.RestoreTransactions((result) =>
                {
                    isRestoreInProgress = false;

                    if (result)
                    {
                        OnRestoreTransactionsFinished();
                    }
                    else
                    {
                        OnRestoreTransactionsFailed("Restoration failed.");
                    }

                    PurchaseLog.Log.LogTrace("RestorePurchases result: " + result + ".");
                });
            }
            else
            {
                OnRestoreTransactionsFailed("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);

                PurchaseLog.Log.LogError("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
            }
        }

        /// <summary>
        /// Start transaction for purchase with specified Id.
        /// </summary>
        /// <param name="purchaseId">Id of an item to buy.</param>
        public void StartPurchaseTransaction(string purchaseId)
        {
            if (IsInitialized())
            {
                Product product = storeController.products.WithID(purchaseId);

                if (product != null && product.availableToPurchase)
                {
                    PurchaseLog.Log.LogTrace("Purchasing product asychronously: '{0}'", product.definition.id);
                    isNewAcqureInProgress = true;
                    storeController.InitiatePurchase(product);
                }
                else
                {
                    PurchaseLog.Log.LogError("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");

                    OnPurchaseTransactionFailed("Not purchasing product, either is not found or is not available for purchase.");
                }
            }
            else
            {
                PurchaseLog.Log.LogError("BuyProduct with id {0} FAIL. Not initialized.", purchaseId);

                OnPurchaseTransactionFailed("Manager is not initialized.");
            }
        }

        /// <summary>
        /// Send request to the e-market server to close current purchase transaction (if there exists one).
        /// </summary>
        public void CloseCurrentPurchaseTransaction()
        {
            if (!HasPendingTransactions)
            {
                PurchaseLog.Log.LogWarning("No transaction to be finished");
                return;
            }

            var product = transactionsQueue.Dequeue();

            PurchaseLog.Log.LogTrace("Finish transaction with id: {0}", product.transactionID);
            AnalyticsManager.Instance.LogRevenue(product);

            storeController.ConfirmPendingPurchase(product);

            OnPurchaseTransactionClosed();
        }

        /// <summary>
        /// Sets aside the purchase transaction, which is currently is waiting for being consumed (if there exists one).
        /// </summary>
        /// <remarks>
        /// It does NOT consumes the purchase but leaves it as the pending one (i.e. transaction will show up the next app launch).
        /// </remarks>
        public void PostponeCurrentPurchaseTransaction()
        {
            if (transactionsQueue.Any())
            {
                transactionsQueue.Dequeue();
            }
        }

        /// <summary>
        /// Start the transaction, which not consumed yet and is waiting for being consumed (if there exists one).
        /// </summary>
        public void StartPendingPurchaseTransaction()
        {
            var product = transactionsQueue.Peek();

            var receiptData = CreateReceipt(product);
            var purchaseTransactionStartedEventArgs = new PurchaseTransactionStartedEventArgs(product.transactionID, receiptData);

            OnPurchaseTransactionStarted(purchaseTransactionStartedEventArgs);

            PurchaseLog.Log.LogTrace("Start finishing pending transactions of {0}", product.transactionID);
        }

        private string CreateReceipt(Product product)
        {
            var receipt = product.receipt;
            var productIdPair = string.Format("\"ProductId\" : \"{0}\",", product.definition.storeSpecificId);
            var index = receipt.IndexOf('{') + 1;
            return receipt.Insert(index, productIdPair);
        }

        private PurchaseRepresentationData CreatePurchase(Product product)
        {
            var id = string.IsNullOrEmpty(product.definition.id) ? string.Empty : product.definition.id;
            var title = string.IsNullOrEmpty(product.metadata.localizedTitle) ? string.Empty : product.metadata.localizedTitle;
            var description = string.IsNullOrEmpty(product.metadata.localizedDescription) ? string.Empty : product.metadata.localizedDescription;
            var currencySymbol = string.IsNullOrEmpty(product.metadata.isoCurrencyCode) ? string.Empty : product.metadata.isoCurrencyCode;

            var purchase = new PurchaseRepresentationData(id, title, description, currencySymbol, product.metadata.localizedPrice, product.transactionID);

            LogPurchaseCreation(purchase, currencySymbol);

            return purchase;
        }

        private bool IsPurchaseValid(PurchaseRepresentationData purchase)
        {
            return !string.IsNullOrEmpty(purchase.Id) &&
                   !string.IsNullOrEmpty(purchase.Title) &&
                   !string.IsNullOrEmpty(purchase.FormatedPrice) &&
                   !string.IsNullOrEmpty(purchase.Description) &&
                   !string.IsNullOrEmpty(purchase.CurrencySymbol);
        }

        private void LogPurchaseCreation(PurchaseRepresentationData purchase, string priceCurrencyCode)
        {
            var log = string.Format("{0}; PriceCurrecyCode: {1}", purchase, priceCurrencyCode);

            if (IsPurchaseValid(purchase))
            {
                PurchaseLog.Log.LogTrace(log);
            }
            else
            {
                PurchaseLog.Log.LogWarning("BAD PURCHASE " + log);
            }
        }

        private ProductType GetProductType(InAppPurchaseType type)
        {
            switch (type)
            {
                case InAppPurchaseType.Consumable:
                    return ProductType.Consumable;

                case InAppPurchaseType.NonConsumable:
                    return ProductType.NonConsumable;

                default:
                    PurchaseLog.Log.LogError("Unrecognized type of in app purchase. Type: {0}", type);
                    return ProductType.Consumable;
            }
        }

        private bool IsInitialized()
        {
            // Only say we are initialized if both the Purchasing references are set.
            return storeController != null && storeExtensionProvider != null;
        }

        private string GetProductName(string sku)
        {
            foreach (var data in purchasesInfo)
            {
                if (data.Id == sku)
                {
                    return data.Title;
                }
            }

            return string.Empty;
        }

        private TransactionType GetTransactionType()
        {
            if (isNewAcqureInProgress)
            {
                return TransactionType.NewAcquire;
            }

            if (isRestoreInProgress)
            {
                return TransactionType.Restore;
            }

            return TransactionType.Confirm;
        }

        #region IStoreListener

        /// <summary>
        /// Called when Unity IAP has retrieved all product metadata and is ready to make purchases.
        /// </summary>
        /// <param name="controller">Access cross-platform Unity IAP functionality.</param>
        /// <param name="extensions">Access store-specific Unity IAP functionality.</param>
        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            // Overall Purchasing system, configured with products for this application.
            storeController = controller;
            // Store specific subsystem, for accessing device-specific store features.
            storeExtensionProvider = extensions;

            purchasesInfo = new List<PurchaseRepresentationData>();
            var products = storeController.products.all.Where(p => p.availableToPurchase);

            foreach (var product in products)
            {
                var data = CreatePurchase(product);

                purchasesInfo.Add(data);
            }

            OnPurchasesLoaded();
        }

        /// <summary>
        /// Note that Unity IAP will not call this method if the device is offline, but continually attempt initialization until online.
        /// </summary>
        /// <param name="error">The reason IAP cannot initialize.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">error - null</exception>
        public void OnInitializeFailed(InitializationFailureReason error)
        {
            switch (error)
            {
                case InitializationFailureReason.PurchasingUnavailable:
                    OnBillingNotSupported();
                    break;
                case InitializationFailureReason.NoProductsAvailable:
                    OnPurchasesFailedToBeLoaded("No products available");
                    break;
                case InitializationFailureReason.AppNotKnown:
                    break;
                default:
                    var message = string.Format("OnInitializeFailed. Hanlder for error : {0} is not implemented.", error);
                    PurchaseLog.Log.LogWarning(message);
                    break;
            }
        }

        /// <summary>
        /// Processes the purchase.
        /// </summary>
        /// <param name="args">The <see cref="PurchaseEventArgs"/> instance containing the event data.</param>
        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            var message = string.Format(
                "ProcessPurchase. Product ID: {0}; Transaction ID: {1}; Receipt: {2}",
                args.purchasedProduct.definition.id,
                args.purchasedProduct.transactionID,
                args.purchasedProduct.receipt);

            PurchaseLog.Log.LogTrace(message);

            transactionsQueue.Enqueue(args.purchasedProduct);

            OnTransactionsQueueIncreased(GetTransactionType());

            isNewAcqureInProgress = false;

            // Wait for server validation.
            return PurchaseProcessingResult.Pending;
        }

        /// <summary>
        /// Called when purchase failed.
        /// </summary>
        /// <param name="product">The product.</param>
        /// <param name="failureReason">The failure reason.</param>
        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            isNewAcqureInProgress = false;

            switch (failureReason)
            {
                case PurchaseFailureReason.UserCancelled:
                    var msg = string.Format("Purchase has been CANCELED. Product Id {0}.", product.definition.id);
                    PurchaseLog.Log.LogTrace(msg);
                    OnPurchaseTransactionCanceled(msg);
                    break;
                case PurchaseFailureReason.PurchasingUnavailable:
                case PurchaseFailureReason.ExistingPurchasePending:
                case PurchaseFailureReason.ProductUnavailable:
                case PurchaseFailureReason.SignatureInvalid:
                case PurchaseFailureReason.PaymentDeclined:
                case PurchaseFailureReason.Unknown:
                    PurchaseLog.Log.LogTrace("Purchase has been FAILED. Product Id {0}. Reason: {1}", product.definition.id, failureReason);
                    OnPurchaseTransactionFailed(failureReason.ToString());
                    break;
                default:
                    PurchaseLog.Log.LogTrace("Purchase has been FAILED. Product Id {0}. Reason: {1}", product.definition.id, failureReason);
                    OnPurchaseTransactionFailed(failureReason.ToString());
                    break;
            }
        }

        #endregion

        #region [Event invokators]

        private void OnTransactionsQueueIncreased(TransactionType transactionType)
        {
            TransactionsQueueIncreased?.Invoke(transactionType);
        }

        private void OnBillingNotSupported()
        {
            BillingIsNotSupported?.Invoke("Purchasing Unavailable");
        }

        private void OnPurchasesLoaded()
        {
            PurchasesLoaded?.Invoke();
        }

        private void OnPurchasesFailedToBeLoaded(string errorMessage)
        {
            PurchasesFailedToBeLoaded?.Invoke(errorMessage);
        }

        private void OnPurchaseTransactionStarted(PurchaseTransactionStartedEventArgs e)
        {
            PurchaseTransactionStarted?.Invoke(e);
        }

        private void OnPurchaseTransactionFailed(string error)
        {
            PurchaseTransactionFailed?.Invoke(error);
        }

        private void OnPurchaseTransactionCanceled(string message)
        {
            PurchaseTransactionCanceled?.Invoke(message);
        }

        private void OnPurchaseTransactionClosed()
        {
            PurchaseTransactionClosed?.Invoke();
        }

        private void OnRestoreTransactionsFinished()
        {
            RestoreTransactionsFinished?.Invoke();
        }

        private void OnRestoreTransactionsFailed(string message)
        {
            RestoreTransactionsFailed?.Invoke(message);
        }

        #endregion
    }
}
