using System.Collections.Generic;
using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Configuration.Data;

namespace Heyworks.PocketShooter.Meta.Entities
{
    public class ShopProductData : ShopProductBase, IProductData
    {
        public ShopProductData(string id, IShopConfiguration shopConfiguration)
            : base(id, shopConfiguration)
        {
            Category = shopConfiguration.GetShopProductCategory(Id);
        }

        public IReadOnlyList<ShopCategory> Category { get; }
    }
}
