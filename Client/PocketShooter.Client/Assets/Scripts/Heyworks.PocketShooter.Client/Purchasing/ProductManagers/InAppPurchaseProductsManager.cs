using System;
using System.Collections.Generic;
using System.Linq;
using Heyworks.PocketShooter.Meta.Communication;
using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.Purchasing.Products;
using Heyworks.PocketShooter.Purchasing.PurchaseManager.CommonEntities;
using Heyworks.PocketShooter.Purchasing.PurchaseManager.Emulator;
using Microsoft.Extensions.Logging;

namespace Heyworks.PocketShooter.Purchasing.ProductManagers
{
    /// <summary>
    /// This object is responsible for creation of <see cref="Product"/> objects, which represent in-app purchases in the game's shop.
    /// </summary>
    public sealed class InAppPurchaseProductsManager : ProductsManager
    {
        #region [Private fields]

        private readonly InAppProductsProvider productsProvider;
        private readonly IPurchaseManager purchaseManager;
        private readonly InAppPurchasesRestorer purchasesRestorer;
        private readonly InAppPendingTransactionsConfirmer transactionsConfirmer;
        private readonly IShopConfiguration config;

        #endregion

        #region [Properties]

        /// <summary>
        /// Gets a value indicating whether the list of purchases is initializes.
        /// </summary>
        public bool ArePurchasesInitialized
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether is product acquiring in progress.
        /// </summary>
        public bool IsProductAcquiringInProgress
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the number of purchases.
        /// </summary>
        /// <remarks> The numb of purchases is actual only if purchases are initialized. </remarks>
        public int PurchasesCount
        {
            get
            {
                return productsProvider.PurchasesCount;
            }
        }

        /// <summary>
        /// Gets the purchases restorer.
        /// </summary>
        public InAppPurchasesRestorer PurchasesRestorer
        {
            get
            {
                return purchasesRestorer;
            }
        }

        #endregion

        #region [Construction and initialization]

        /// <summary>
        /// Initializes a new instance of the <see cref="InAppPurchaseProductsManager"/> class.
        /// </summary>
        public InAppPurchaseProductsManager(IGameHubClient shopComponent, IShopConfiguration config, MetaGame game)
        {
            this.config = config;

            purchaseManager = CreatePurchaseManager();
            purchasesRestorer = new InAppPurchasesRestorer(purchaseManager,  shopComponent);
            transactionsConfirmer = new InAppPendingTransactionsConfirmer(purchaseManager,  shopComponent);

            productsProvider = new InAppProductsProvider(purchaseManager, shopComponent, config, game);

            AddEventHandlers();
        }

        #endregion

        #region [Public methods]

        /// <summary>
        /// Initialize purchases.
        /// </summary>
        public void InitializePurchaseManager()
        {
            purchaseManager.BillingIsNotSupported += PurchaseManager_BillingIsNotSupported;
            purchaseManager.PurchasesLoaded += PurchaseManager_PurchasesLoaded;
            purchaseManager.PurchasesFailedToBeLoaded += PurchaseManager_PurchasesFailedToBeLoaded;

            var purchases = config.GetPurchases();
            purchaseManager.Initialize(purchases);
        }

        /// <summary>
        /// Confirms the pending purchases.
        /// </summary>
        public void ConfirmPendingPurchases()
        {
            PurchaseLog.Log.LogTrace("Shop. Finish pending purchases.");
            transactionsConfirmer.FinishPendingPurchases();
        }

        /// <summary>
        /// Get the list of <see cref="Product"/> objects.
        /// </summary>
        public override IEnumerable<Product> GetProducts(Predicate<Product> match = null)
        {
            var shopProducts = productsProvider.GetProducts();

            if (match != null)
            {
                var filteredProducts = new List<Product>(shopProducts.Count());

                foreach (var product in shopProducts)
                {
                    if (match(product))
                    {
                        filteredProducts.Add(product);
                    }
                }

                return filteredProducts;
            }

            return shopProducts;
        }

        /// <summary>
        /// Refreshes the products.
        /// </summary>
        public void UpdateProducts()
        {
            productsProvider.UpdateProductsList();
            OnProductsUpdated();
        }

        #endregion

        #region [Event handlers]

        private void ProductsProvider_ProductAcquiringFinished()
        {
            ProcessAcquiringFinish();
            IsProductAcquiringInProgress = false;
        }

        private void ProductsProvider_ProductAcquiringStarted()
        {
            IsProductAcquiringInProgress = true;
            OnProductAcquiringStarted();
        }

        private void PurchaseManager_BillingIsNotSupported(string message)
        {
            ArePurchasesInitialized = true;

            PurchaseLog.Log.LogTrace("Shop. Billing is not supported.");

            OnProductsUpdated();
        }

        private void PurchaseManager_PurchasesFailedToBeLoaded(string errorMessage)
        {
            ArePurchasesInitialized = true;
            PurchaseLog.Log.LogTrace("Shop. Purchases load failed.");

            OnProductsUpdated();
        }

        private void PurchaseManager_PurchasesLoaded()
        {
            PurchaseLog.Log.LogTrace("Shop. Purchases are loaded successfully.");

            ArePurchasesInitialized = true;
            UpdateProducts();
        }

        private void PurchasesRestorer_ProductAcquiringFinished()
        {
            ProcessAcquiringFinish();
        }

        private void TransactionsConfirmer_ProductAcquiringFinished()
        {
            ProcessAcquiringFinish();
        }

        #endregion

        #region [Private methods]

        private void AddEventHandlers()
        {
            purchasesRestorer.ProductAcquiringFinished += PurchasesRestorer_ProductAcquiringFinished;
            transactionsConfirmer.ProductAcquiringFinished += TransactionsConfirmer_ProductAcquiringFinished;
            productsProvider.ProductAcquiringStarted += ProductsProvider_ProductAcquiringStarted;
            productsProvider.ProductAcquiringFinished += ProductsProvider_ProductAcquiringFinished;
        }

        private IPurchaseManager CreatePurchaseManager()
        {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_IOS
            return new EmulatorPurchaseManager(config.GetPurchases());
#else
        return new UnifiedPurchaseManager();
#endif
        }

        private void ProcessAcquiringFinish()
        {
            if (!purchaseManager.HasPendingTransactions)
            {
                OnProductAcquiringFinished();
                UpdateProducts();
            }
        }

        #endregion
    }
}
