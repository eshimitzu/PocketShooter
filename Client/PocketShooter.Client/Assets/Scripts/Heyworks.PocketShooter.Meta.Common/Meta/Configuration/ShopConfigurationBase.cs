using System.Collections.Generic;
using System.Linq;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Meta.Entities;

namespace Heyworks.PocketShooter.Meta.Configuration
{
    public class ShopConfigurationBase : IShopConfigurationBase
    {
        private readonly GameConfig gameConfig;

        public ShopConfigurationBase(GameConfig gameConfig)
        {
            this.gameConfig = gameConfig;
        }

        /// <summary>
        /// Returns content of in-app purchase.
        /// </summary>
        /// <param name="purchaseId">The purchase id.</param>
        public IEnumerable<IContentIdentity> GetPurchaseContent(string purchaseId)
        {
            var realCurrencyProduct = GetPurchaseProduct(purchaseId);
            var contentPackConfig = GetContentPackConfig(realCurrencyProduct.ContentPackId);

            return contentPackConfig.Content;
        }

        /// <summary>
        /// Returns the price of a roster product.
        /// </summary>
        /// <param name="productId">The product id.</param>
        public Price GetRosterProductPrice(string productId) =>
            GetRosterProductConfig(productId).Price;

        /// <summary>
        /// Returns the player's level required for roster product unlock.
        /// </summary>
        /// <param name="productId">The product id.</param>
        public int GetPlayerLevelForUnlockRosterProduct(string productId) =>
            GetRosterProductConfig(productId).PlayerLevelRequired;

        /// <summary>
        /// Returns the price of a shop product.
        /// </summary>
        /// <param name="productId">The product id.</param>
        public Price GetShopProductPrice(string productId) =>
            GetShopProductConfig(productId).Price;

        /// <summary>
        /// Get minimum required player level for shop product.
        /// </summary>
        /// <param name="productId">The product id.</param>
        public int GetShopProductMinPlayerLevel(string productId) =>
            GetShopProductConfig(productId).MinPlayerLevel;

        /// <summary>
        /// Get maximum valid player level for shop product.
        /// </summary>
        /// <param name="productId">The product id.</param>
        public int GetShopProductMaxPlayerLevel(string productId) =>
            GetShopProductConfig(productId).MaxPlayerLevel;

        /// <summary>
        /// Returns content of roster product.
        /// </summary>
        /// <param name="productId">The product id.</param>
        public IContentIdentity GetRosterProductContent(string productId)
        {
            var rosterProductConfig = GetRosterProductConfig(productId);

            var contentPackConfig = GetContentPackConfig(rosterProductConfig.ContentPackId);
            if (contentPackConfig.Content.Count != 1)
            {
                throw new ConfigurationException($"Roster content pack must contain only one item, found {contentPackConfig.Content.Count}");
            }

            return contentPackConfig.Content[0];
        }

        /// <summary>
        /// Returns content of roster product.
        /// </summary>
        /// <param name="productId">The product id.</param>
        public IEnumerable<IContentIdentity> GetShopProductContent(string productId)
        {
            var shopProductConfig = GetShopProductConfig(productId);

            return GetContentPackConfig(shopProductConfig.ContentPackId).Content;
        }

        /// <summary>
        /// Gets purchase by id.
        /// </summary>
        /// <param name="purchaseId">The in-app purchase id.</param>
        public InAppPurchase GetPurchase(string purchaseId)
        {
            var purchase = FindInAppPurchase(purchaseId);

            if (purchase == null)
            {
                throw new ConfigurationException($"Can't find purchase with id {purchaseId}");
            }

            return purchase;
        }

        /// <summary>
        /// Gets true if roster product exists.
        /// </summary>
        /// <param name="productId">The roster product id.</param>
        public bool IsRosterProductExists(string productId) => FindRosterProductConfig(productId) != null;

        /// <summary>
        /// Gets true if shop product exists.
        /// </summary>
        /// <param name="productId">The shop product id.</param>
        public bool IsShopProductExists(string productId) => FindShopProductConfig(productId) != null;

        /// <summary>
        /// Gets true if purchase exists.
        /// </summary>
        /// <param name="purchaseId">The in-app purchase id.</param>
        public bool IsPurchaseExists(string purchaseId) => FindInAppPurchase(purchaseId) != null;

        protected ShopProductConfig GetShopProductConfig(string productId)
        {
            var shopProductConfig = FindShopProductConfig(productId);
            if (shopProductConfig == null)
            {
                throw new ConfigurationException($"Can't find shop product config with id {productId}");
            }

            return shopProductConfig;
        }

        private (string Id, Price Price, string ContentPackId) GetPurchaseProduct(string purchaseId)
        {
            var realCurrencyProduct =
                            gameConfig.RosterProductsConfig.Select(_ => (_.Id, _.Price, _.ContentPackId))
                            .Concat(gameConfig.ShopProductsConfig.Select(_ => (_.Id, _.Price, _.ContentPackId)))
                            .SingleOrDefault(item => item.Price.Type == PriceType.RealCurrency && item.Price.PurchaseId == purchaseId);

            if (realCurrencyProduct == default)
            {
                throw new ConfigurationException($"Can't find real currency product config for purchase with id {purchaseId}");
            }

            return realCurrencyProduct;
        }

        private RosterProductConfig FindRosterProductConfig(string productId) =>
            gameConfig.RosterProductsConfig.SingleOrDefault(_ => _.Id == productId);

        private RosterProductConfig GetRosterProductConfig(string productId)
        {
            var rosterProductConfig = FindRosterProductConfig(productId);
            if (rosterProductConfig == null)
            {
                throw new ConfigurationException($"Can't find roster product config with id {productId}");
            }

            return rosterProductConfig;
        }

        private ContentPackConfig GetContentPackConfig(string contentPackId)
        {
            var contentPackConfig = gameConfig.ContentPacksConfig.SingleOrDefault(_ => _.Id == contentPackId);
            if (contentPackConfig == null)
            {
                throw new ConfigurationException($"Can't find content pack config with id {contentPackId}");
            }

            return contentPackConfig;
        }

        private ShopProductConfig FindShopProductConfig(string productId) =>
            gameConfig.ShopProductsConfig.SingleOrDefault(_ => _.Id == productId);

        private InAppPurchase FindInAppPurchase(string purchaseId) =>
            gameConfig.InAppPurchasesConfig.SingleOrDefault(p => p.Id == purchaseId);
    }
}