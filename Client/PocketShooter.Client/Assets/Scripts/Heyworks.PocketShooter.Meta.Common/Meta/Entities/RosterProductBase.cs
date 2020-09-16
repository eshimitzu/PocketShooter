using Heyworks.PocketShooter.Meta.Configuration;

namespace Heyworks.PocketShooter.Meta.Entities
{
    public class RosterProductBase
    {
        public RosterProductBase(string id, IShopConfigurationBase shopConfiguration)
        {
            Id = id;

            Price = shopConfiguration.GetRosterProductPrice(Id);
            Content = shopConfiguration.GetRosterProductContent(Id);
            PlayerLevelForUnlock = shopConfiguration.GetPlayerLevelForUnlockRosterProduct(Id);
        }

        public string Id { get; }

        public Price Price { get; } 

        public IContentIdentity Content { get; }

        public int PlayerLevelForUnlock { get; }
    }
}
