using System;
using System.Collections.Generic;
using Heyworks.PocketShooter.Meta.Communication;
using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.Purchasing.Products;
using Microsoft.Extensions.Logging;

namespace Heyworks.PocketShooter.Purchasing.PurchaseManager.CommonEntities
{
    /// <summary>
    /// Represents provider for in-app shop products.
    /// </summary>
    public class InAppProductsProvider
    {
        #region [Private fields]

        private readonly IShopConfiguration config;
        private readonly List<Product> products = new List<Product>();
        private readonly IPurchaseManager purchaseManager;
        private readonly IGameHubClient shopComponent;
        private readonly MetaGame game;

        #endregion

        #region [Events]

        /// <summary>
        /// Event fires when some product acquiring has just been finished either successfully or not.
        /// </summary>
        public event Action ProductAcquiringFinished;

        /// <summary>
        /// Event fires when acquiring of some product has just started.
        /// </summary>
        public event Action ProductAcquiringStarted;

        #endregion

        #region [Properties]

        /// <summary>
        /// Gets the number of purchases.
        /// </summary>
        /// <remarks> The numb of purchases is actual only if purchases are initialized. </remarks>
        public int PurchasesCount
        {
            get
            {
                return products.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether there exist at least one product of purchasing gold multiplier.
        /// </summary>
        public bool HasMultiplierProducts
        {
            get;
            private set;
        }

        #endregion

        #region [Construction and initialization]

        public InAppProductsProvider(IPurchaseManager purchaseManager, IGameHubClient shopComponent, IShopConfiguration config, MetaGame game)
        {
            this.config = config;
            this.shopComponent = shopComponent;
            this.purchaseManager = purchaseManager;
            this.game = game;
        }

        #endregion

        #region [Public methods]

        /// <summary>
        /// Get the list of <see cref="Product"/> objects.
        /// </summary>
        public IEnumerable<Product> GetProducts()
        {
            var availableShopProducts = new Product[products.Count];
            var i = 0;

            foreach (var product in products)
            {
                availableShopProducts[i] = product;
                i++;
            }

            return availableShopProducts;
        }

        public void UpdateProductsList()
        {
            var purchasesData = purchaseManager.GetPurchases();
            ClearProductsList();
            foreach (var purchaseRepresentationData in purchasesData)
            {
                if (!IsPurchaseValid(purchaseRepresentationData.Id))
                {
                    continue;
                }

                var inAppPurchaseData = config.GetPurchase(purchaseRepresentationData.Id);
                var product = config.GetProductByPurchaseId(purchaseRepresentationData.Id);

                CreateShopProduct(product, purchaseRepresentationData, inAppPurchaseData);
            }

            PurchaseLog.Log.LogTrace("Updating products list. Number of products {0}", products.Count);
        }

        #endregion

        #region [Event handlers]

        private void ShopProductAcquiring_AcquireFinished(IProductAcquiring args)
        {
            OnProductAcquiringFinished();
        }

        private void ShopProductAcquiring_AcquiringStarted(IProductAcquiring args)
        {
            OnProductAcquiringStarted();
        }

        #endregion

        #region [Private methods]

        private void AddProductEventHandlers(IProductAcquiring product)
        {
            product.AcquiringStarted += ShopProductAcquiring_AcquiringStarted;
            product.AcquireFailed += ShopProductAcquiring_AcquireFinished;
            product.AcquireSucceeded += ShopProductAcquiring_AcquireFinished;
        }

        private void ClearProductsList()
        {
            foreach (var product in products)
            {
                RemoveProductEventHandlers(product);
            }

            products.Clear();
        }

        private void CreateShopProduct(
            IProductData productData,
            PurchaseRepresentationData purchaseRepresentationData,
            InAppPurchase inAppPurchaseData)
        {
            Product product = null;

            switch (productData)
            {
                case RosterProductData rosterProductData:
                    product = new InAppPurchaseRosterProduct(
                        purchaseManager,
                        inAppPurchaseData,
                        purchaseRepresentationData,
                        rosterProductData,
                        shopComponent,
                        game);
                    break;
                case ShopProductData shopProductData:
                    product = new InAppPurchaseShopProduct(
                        purchaseManager,
                        inAppPurchaseData,
                        purchaseRepresentationData,
                        shopProductData,
                        shopComponent,
                        game);
                    break;
                default:
                    throw new InvalidOperationException($"Can't create ina app shop product. Type '{productData.GetType()}' is not supported.");

            }

            AddProductEventHandlers(product);
            products.Add(product);
        }

        private bool IsPurchaseValid(string id)
        {
            return config.IsPurchaseExists(id);
        }

        private void OnProductAcquiringFinished()
        {
            var handler = ProductAcquiringFinished;
            if (handler != null)
            {
                handler();
            }
        }

        private void OnProductAcquiringStarted()
        {
            var handler = ProductAcquiringStarted;
            if (handler != null)
            {
                handler();
            }
        }

        private void RemoveProductEventHandlers(IProductAcquiring product)
        {
            product.AcquiringStarted -= ShopProductAcquiring_AcquiringStarted;
            product.AcquireFailed -= ShopProductAcquiring_AcquireFinished;
            product.AcquireSucceeded -= ShopProductAcquiring_AcquireFinished;
        }

        #endregion
    }
}