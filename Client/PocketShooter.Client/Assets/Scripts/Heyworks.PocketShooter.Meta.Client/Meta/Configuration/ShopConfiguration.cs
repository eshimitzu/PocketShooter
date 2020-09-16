using System.Collections.Generic;
using System.Linq;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Meta.Entities;

namespace Heyworks.PocketShooter.Meta.Configuration
{
    public class ShopConfiguration : ShopConfigurationBase, IShopConfiguration
    {
        private readonly GameConfig gameConfig;

        public ShopConfiguration(GameConfig gameConfig)
            : base(gameConfig)
        {
            this.gameConfig = gameConfig;
        }

        /// <summary>
        /// Returns shop category of the shop product.
        /// </summary>
        /// <param name="productId">The product id.</param>
        public IReadOnlyList<ShopCategory> GetShopProductCategory(string productId) =>
            GetShopProductConfig(productId).Category.ToList();

        public IReadOnlyList<RosterProductData> GetRosterProducts() =>
            gameConfig.RosterProductsConfig.Select(_ => new RosterProductData(_.Id, this)).ToList();

        public IReadOnlyList<ShopProductData> GetShopProducts() =>
            gameConfig.ShopProductsConfig.Select(_ => new ShopProductData(_.Id, this)).ToList();

        public IProductData GetProductByPurchaseId(string purchaseId)
        {
            var realCurrencyRosterProduct =
                gameConfig.RosterProductsConfig.SingleOrDefault(item => item.Price.Type == PriceType.RealCurrency && item.Price.PurchaseId == purchaseId);

            if (realCurrencyRosterProduct != null)
            {
                return new RosterProductData(realCurrencyRosterProduct.Id, this);
            }

            var realCurrencyShopProduct =
                gameConfig.ShopProductsConfig.SingleOrDefault(item => item.Price.Type == PriceType.RealCurrency && item.Price.PurchaseId == purchaseId);

            if (realCurrencyShopProduct != null)
            {
                return new ShopProductData(realCurrencyShopProduct.Id, this);
            }

            throw new ConfigurationException($"Can't find shop product config with purchase id {purchaseId}");
        }

        public IReadOnlyList<InAppPurchase> GetPurchases() => gameConfig.InAppPurchasesConfig.ToList();
    }
}