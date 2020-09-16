using System.Collections.Generic;
using Heyworks.PocketShooter.Meta.Entities;

namespace Heyworks.PocketShooter.Meta.Configuration
{
    public interface IShopConfigurationBase
    {
        /// <summary>
        /// Returns content of in-app purchase.
        /// </summary>
        /// <param name="purchaseId">The purchase id.</param>
        IEnumerable<IContentIdentity> GetPurchaseContent(string purchaseId);

        /// <summary>
        /// Returns the price of a roster product.
        /// </summary>
        /// <param name="productId">The product id.</param>
        Price GetRosterProductPrice(string productId);

        /// <summary>
        /// Returns the player's level required for roster product unlock.
        /// </summary>
        /// <param name="productId">The product id.</param>
        int GetPlayerLevelForUnlockRosterProduct(string productId);

        /// <summary>
        /// Returns the price of a shop product.
        /// </summary>
        /// <param name="productId">The product id.</param>
        Price GetShopProductPrice(string productId);

        /// <summary>
        /// Get minimum required player level for shop product.
        /// </summary>
        /// <param name="productId">The product id.</param>
        int GetShopProductMinPlayerLevel(string productId);

        /// <summary>
        /// Get maximum valid player level for shop product.
        /// </summary>
        /// <param name="productId">The product id.</param>
        int GetShopProductMaxPlayerLevel(string productId);

        /// <summary>
        /// Returns content of roster product.
        /// </summary>
        /// <param name="productId">The product id.</param>
        IContentIdentity GetRosterProductContent(string productId);

        /// <summary>
        /// Returns content of shop product.
        /// </summary>
        /// <param name="productId">The product id.</param>
        IEnumerable<IContentIdentity> GetShopProductContent(string productId);

        /// <summary>
        /// Gets purchase by id.
        /// </summary>
        /// <param name="purchaseId">The in-app purchase id.</param>
        InAppPurchase GetPurchase(string purchaseId);

        /// <summary>
        /// Gets true if roster product exists.
        /// </summary>
        /// <param name="productId">The roster product id.</param>
        bool IsRosterProductExists(string productId);

        /// <summary>
        /// Gets true if shop product exists.
        /// </summary>
        /// <param name="productId">The shop product id.</param>
        bool IsShopProductExists(string productId);

        /// <summary>
        /// Gets true if purchase exists.
        /// </summary>
        /// <param name="purchaseId">The in-app purchase id.</param>
        bool IsPurchaseExists(string purchaseId);
    }
}