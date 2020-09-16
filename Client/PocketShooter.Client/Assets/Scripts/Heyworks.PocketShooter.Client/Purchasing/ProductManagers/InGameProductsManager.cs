using System;
using System.Collections.Generic;
using System.Linq;
using Heyworks.PocketShooter.Meta.Communication;
using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.Purchasing.Products;

namespace Heyworks.PocketShooter.Purchasing.ProductManagers
{
    /// <summary>
    /// This class provides functionality to get collection of <see cref="Product"/> objects. It manages not in app products.
    /// </summary>
    public class InGameProductsManager : ProductsManager
    {
        private readonly List<Product> shopProducts = new List<Product>();
        private readonly IGameHubClient shopComponent;
        private readonly IShopConfiguration config;
        private readonly MetaGame game;

        /// <summary>
        /// Initializes a new instance of the <see cref="InGameProductsManager"/> class.
        /// </summary>
        public InGameProductsManager(IGameHubClient shopComponent, IShopConfiguration config, MetaGame game)
        {
            this.shopComponent = shopComponent;
            this.config = config;
            this.game = game;

            InitializeProducts();
        }

        /// <summary>
        /// Manually runs the process of updating of list of instances of <see cref="Product"/> class,
        /// which can be received from the manager.
        /// </summary>
        protected override void Update()
        {
            InitializeProducts();
            OnProductsUpdated();
        }

        /// <summary>
        /// Get the list of <see cref="Product"/> objects.
        /// </summary>
        public override IEnumerable<Product> GetProducts(Predicate<Product> match = null)
        {
            if (match != null)
            {
                var filteredProducts = new List<Product>(shopProducts.Count);

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

        private void InitializeProducts()
        {
            ClearProductsList();

            var rosterProducts = config.GetRosterProducts().Where(p => p.Price.Type != PriceType.RealCurrency);
            foreach (var product in rosterProducts)
            {
                var rosterProduct = CreateRosterContentInGameProduct(product);

                shopProducts.Add(rosterProduct);
                AddProductAcquiringEventHandlers(rosterProduct);
            }

            var inShopProducts = config.GetShopProducts().Where(p => p.Price.Type != PriceType.RealCurrency);
            foreach (var product in inShopProducts)
            {
                var inShopProduct = CreateShopContentInGameProduct(product);

                shopProducts.Add(inShopProduct);
                AddProductAcquiringEventHandlers(inShopProduct);
            }
        }

        private InGameRosterProduct CreateRosterContentInGameProduct(RosterProductData rosterProduct)
        {
            return new InGameRosterProduct(rosterProduct, shopComponent, game);
        }

        private InGameShopProduct CreateShopContentInGameProduct(ShopProductData shopProductData)
        {
            return new InGameShopProduct(shopProductData, shopComponent, game);
        }

        private void ClearProductsList()
        {
            shopProducts.Clear();
        }

        private void AddProductAcquiringEventHandlers(IProductAcquiring shopProductAcquiring)
        {
            shopProductAcquiring.AcquiringStarted += ShopProductAcquiring_AcquiringStarted;
            shopProductAcquiring.AcquireFailed += ShopProductAcquiring_AcquireFinished;
            shopProductAcquiring.AcquireSucceeded += ShopProductAcquiring_AcquireFinished;
        }

        private void ShopProductAcquiring_AcquiringStarted(IProductAcquiring shopProductAcquiring)
        {
            OnProductAcquiringStarted();
        }

        private void ShopProductAcquiring_AcquireFinished(IProductAcquiring shopProductAcquiring)
        {
            OnProductAcquiringFinished();
            Update();
        }
    }
}
