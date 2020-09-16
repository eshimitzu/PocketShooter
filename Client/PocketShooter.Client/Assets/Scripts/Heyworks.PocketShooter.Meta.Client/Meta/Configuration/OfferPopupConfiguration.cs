using System.Collections.Generic;
using System.Linq;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Meta.Entities;

namespace Heyworks.PocketShooter.Meta.Configuration
{
    public class OfferPopupConfiguration : IOfferPopupConfiguration
    {
        private readonly GameConfig gameConfig;

        public OfferPopupConfiguration(GameConfig gameConfig)
        {
            this.gameConfig = gameConfig;
        }

        public IReadOnlyList<OfferPopupData> GetOfferPopups() =>
            gameConfig.OfferPopupConfig.Select(_ => new OfferPopupData(_.OfferProductId, _.AppearanceChance, _.Discount)).ToList();
    }
}