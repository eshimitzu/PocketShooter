using System.Collections.Generic;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Meta.Entities;

namespace Heyworks.PocketShooter.Meta.Configuration
{
    public interface IShopConfiguration : IShopConfigurationBase
    {
        /// <summary>
        /// Returns shop category of the shop product.
        /// </summary>
        /// <param name="productId">The product id.</param>
        IReadOnlyList<ShopCategory> GetShopProductCategory(string productId);

        IReadOnlyList<InAppPurchase> GetPurchases();

        IReadOnlyList<RosterProductData> GetRosterProducts();

        IReadOnlyList<ShopProductData> GetShopProducts();

        IProductData GetProductByPurchaseId(string purchaseId);
    }
}