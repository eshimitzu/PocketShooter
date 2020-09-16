using System.Collections.Generic;
using Heyworks.PocketShooter.Meta.Configuration;

namespace Heyworks.PocketShooter.Meta.Entities
{
    public class ShopProductBase
    {
        public ShopProductBase(string id, IShopConfigurationBase shopConfiguration)
        {
            Id = id;

            Price = shopConfiguration.GetShopProductPrice(Id);
            Content = shopConfiguration.GetShopProductContent(Id);
            MinPlayerLevel = shopConfiguration.GetShopProductMinPlayerLevel(Id);
            MaxPlayerLevel = shopConfiguration.GetShopProductMaxPlayerLevel(Id);
        }
        
        public string Id { get; }

        public Price Price { get; }

        public IEnumerable<IContentIdentity> Content { get; }

        public int MinPlayerLevel { get; }

        public int MaxPlayerLevel { get; }
    }
}
