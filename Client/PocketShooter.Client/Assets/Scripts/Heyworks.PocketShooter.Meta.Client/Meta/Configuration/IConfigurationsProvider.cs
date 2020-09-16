namespace Heyworks.PocketShooter.Meta.Configuration
{
    public interface IConfigurationsProvider
    {
        IPlayerConfigurationBase PlayerConfiguration { get; }

        IArmyConfigurationBase ArmyConfiguration { get; }

        ITrooperConfigurationBase TrooperConfiguration { get; }

        IWeaponConfigurationBase WeaponConfiguration { get; }

        IArmorConfigurationBase ArmorConfiguration { get; }

        IHelmetConfigurationBase HelmetConfiguration { get; }

        IShopConfiguration ShopConfiguration { get; }

        IOfferPopupConfiguration OfferPopupConfiguration { get; }
    }
}
