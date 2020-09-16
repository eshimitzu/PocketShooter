namespace Heyworks.PocketShooter.Meta.Configuration
{
    public class ConfigurationsProvider : IConfigurationsProvider
    {
        private readonly IConfigurationStorage configurationStorage;

        public ConfigurationsProvider(IConfigurationStorage configurationStorage)
        {
            this.configurationStorage = configurationStorage;
        }

        public IPlayerConfigurationBase PlayerConfiguration =>
            new PlayerConfigurationBase(configurationStorage.GameConfig);

        public IArmyConfigurationBase ArmyConfiguration =>
            new ArmyConfigurationBase(configurationStorage.GameConfig);

        public ITrooperConfigurationBase TrooperConfiguration =>
            new TrooperConfigurationBase(configurationStorage.GameConfig);

        public IWeaponConfigurationBase WeaponConfiguration =>
            new WeaponConfigurationBase(configurationStorage.GameConfig);

        public IArmorConfigurationBase ArmorConfiguration =>
            new ArmorConfigurationBase(configurationStorage.GameConfig);            

        public IShopConfiguration ShopConfiguration => new ShopConfiguration(configurationStorage.GameConfig);

        public IHelmetConfigurationBase HelmetConfiguration => new HelmetConfigurationBase(configurationStorage.GameConfig);

        public IOfferPopupConfiguration OfferPopupConfiguration => new OfferPopupConfiguration(configurationStorage.GameConfig);
    }
}
