using System.Linq;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Purchasing.Products;

namespace Heyworks.PocketShooter.Purchasing.Core
{
    public static class ShopExtension
    {
        public static ShopProduct[] GetVisibleShopProductsWithCategory(this Shop shop, ShopCategory shopCategory)
        {
            return shop.GetShopProducts(p =>
                p is ShopProduct shopProduct &&
                shopProduct.Category.Contains(shopCategory) &&
                shopProduct.IsVisible)
                .Select(p => (ShopProduct)p).ToArray();
        }
    }
}