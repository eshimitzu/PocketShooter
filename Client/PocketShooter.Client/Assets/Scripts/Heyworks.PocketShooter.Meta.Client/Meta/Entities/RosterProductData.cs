using Heyworks.PocketShooter.Meta.Configuration;

namespace Heyworks.PocketShooter.Meta.Entities
{
    public class RosterProductData : RosterProductBase, IProductData
    {
        public RosterProductData(string id, IShopConfigurationBase shopConfiguration)
            : base(id, shopConfiguration)
        {
        }
    }
}