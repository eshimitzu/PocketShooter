using System;
using System.Collections.Generic;
using Heyworks.PocketShooter.Purchasing.ProductManagers;
using Heyworks.PocketShooter.Purchasing.Products;

namespace Heyworks.PocketShooter.Purchasing.Core
{
    /// <summary>
    /// This class encapsulates collection of all products, which can be acquired in the game.
    /// </summary>
    public sealed class Shop
    {
        #region [Private fields]

        private readonly InAppPurchaseProductsManager inAppProductsManager;
        private readonly List<ProductsManager> productsManagers;

        #endregion

        #region [Events]

        /// <summary>
        /// Event fires when acquiring of some product has just started.
        /// </summary>
        public event Action ProductAcquiringStarted;

        /// <summary>
        /// Event fires when some product acquiring has just been finished either successfully or not.
        /// </summary>
        public event Action ProductAcquiringFinished;

        /// <summary>
        /// Event fires when the list of all available shop products has updated.
        /// </summary>
        public event Action ProductsListUpdated;

        /// <summary>
        /// Fired when all transactions from the user's purchase history have successfully been added back to the queue.
        /// </summary>
        public event Action RestorePurchasesFinished;

        /// <summary>
        /// Fired when an error is encountered while adding transactions from the user's purchase history back to the queue.
        /// </summary>
        public event Action RestorePurchasesFailed;

        #endregion

        #region [Properties]

        /// <summary>
        /// Gets a value indicating whether there exist purchases in the shop.
        /// </summary>
        public bool HasPurchases
        {
            get
            {
                return inAppProductsManager.PurchasesCount > 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the request to get in-app purchases is successfully handled.
        /// </summary>
        public bool ArePurchasesInitialized
        {
            get
            {
                return inAppProductsManager.ArePurchasesInitialized;
            }
        }

        #endregion

        #region [Constructor]

        /// <summary>
        /// Initializes a new instance of the <see cref="Shop"/> class.
        /// </summary>
        /// <param name="inAppProductsManager"> Object, which encapsulates creation of shop products, encapsulating in-app purchases. </param>
        /// <param name="nonInAppProductsManagers"> Object, which encapsulates creation of shop products, which are not in-app purchases. </param>
        public Shop(InAppPurchaseProductsManager inAppProductsManager, IList<ProductsManager> nonInAppProductsManagers)
        {
            this.inAppProductsManager = inAppProductsManager;

            var purchasesRestorer = inAppProductsManager.PurchasesRestorer;
            purchasesRestorer.RestoreTransactionsFinished += PurchasesRestorer_RestoreTransactionsFinished;
            purchasesRestorer.RestoreTransactionsFailed += PurchasesRestorer_RestoreTransactionsFailed;
            purchasesRestorer.RestoreTransactionsStarted += PurchasesRestorer_RestoreTransactionsStarted;

            productsManagers = new List<ProductsManager>(nonInAppProductsManagers)
            {
                inAppProductsManager,
            };

            foreach (var productsManager in productsManagers)
            {
                productsManager.ProductAcquiringStarted += ProductsManager_ProductAcquiringStarted;
                productsManager.ProductAcquiringFinished += ProductsManager_ProductAcquiringFinished;
                productsManager.ProductsUpdated += ProductsManager_ProductsUpdated;
            }
        }

        #endregion

        #region [Public methods]

        /// <summary>
        /// Initialize purchases.
        /// </summary>
        public void InitializeInAppPurchases()
        {
            inAppProductsManager.InitializePurchaseManager();
        }

        /// <summary>
        /// Restores all non-consumable purchases.
        /// </summary>
        public void RestorePurchases()
        {
            inAppProductsManager.PurchasesRestorer.RestorePurchases();
        }

        /// <summary>
        /// Confirms the pending in-app purchases.
        /// </summary>
        public void ConfirmPendingInAppPurchases()
        {
            inAppProductsManager.ConfirmPendingPurchases();
        }

        /// <summary>
        /// Get the list of all shop products matching the specified condition.
        /// </summary>
        /// <param name="match"> Condition which all returned shop products must satisfy. </param>
        public IList<Product> GetShopProducts(Predicate<Product> match = null)
        {
            var products = new List<Product>();

            foreach (var productsManager in productsManagers)
            {
                products.AddRange(productsManager.GetProducts());
            }

            return match == null ? products : products.FindAll(match);
        }

        #endregion

        #region [Event handlers & invokators]

        private void Offers_Updated()
        {
            if (!inAppProductsManager.IsProductAcquiringInProgress)
            {
                inAppProductsManager.UpdateProducts();
            }
        }

        private void ProductsManager_ProductAcquiringStarted()
        {
            OnProductAcquiringStarted();
        }

        private void ProductsManager_ProductAcquiringFinished()
        {
            OnProductAcquiringFinished();
        }

        private void ProductsManager_ProductsUpdated()
        {
            OnProductsListUpdated();
        }

        private void PurchasesRestorer_RestoreTransactionsFailed()
        {
            OnRestorePurchasesFailed();
        }

        private void PurchasesRestorer_RestoreTransactionsStarted()
        {
            OnProductAcquiringStarted();
        }

        private void PurchasesRestorer_RestoreTransactionsFinished()
        {
            OnRestorePurchasesFinished();
        }

        private void OnProductAcquiringStarted()
        {
            var handler = ProductAcquiringStarted;
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

        private void OnProductsListUpdated()
        {
            var handler = ProductsListUpdated;
            if (handler != null)
            {
                handler();
            }
        }

        private void OnRestorePurchasesFinished()
        {
            var handler = RestorePurchasesFinished;
            if (handler != null)
            {
                handler();
            }
        }

        private void OnRestorePurchasesFailed()
        {
            var handler = RestorePurchasesFailed;
            if (handler != null)
            {
                handler();
            }
        }

        #endregion
    }
}
