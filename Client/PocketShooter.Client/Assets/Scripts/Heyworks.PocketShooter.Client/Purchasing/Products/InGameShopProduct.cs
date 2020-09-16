using System.Linq;
using Heyworks.PocketShooter.Meta.Communication;
using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.Modules.Analytics;
using Heyworks.PocketShooter.Purchasing.AcquiringFlows;
using Heyworks.PocketShooter.UI;
using Heyworks.PocketShooter.UI.Localization;

namespace Heyworks.PocketShooter.Purchasing.Products
{
    public class InGameShopProduct : ShopProduct
    {
        /// <summary>
        /// Gets localized name of the shop's product.
        /// </summary>
        public override string Name => LocKeys.GetShopItemKey(Id).Localized();

        /// <summary>
        /// Initializes a new instance of the <see cref="InGameShopProduct"/> class.
        /// </summary>
        public InGameShopProduct(
            ShopProductData shopProductData,
            IGameHubClient shopComponent,
            MetaGame game)
            : base(shopProductData, shopComponent, game)
        {
        }

        /// <summary>
        /// Gets the acquiring flow of the shop product.
        /// </summary>
        protected override ShopProductAcquiringFlow CreateAcquiringFlow()
        {
            return new InGameShopProductAcquiringFlow(ShopComponent, Id);
        }

        protected override void OnAcquiringSucceeded()
        {
            AnalyticsManager.Instance.SendInGamePurchase(Id, Price, Content, Category);
            AnalyticsManager.Instance.SendCurrencyIncomingFromShop(Id, Content, Category);

            if (Content.Count() > 1)
            {
                foreach (var contentIdentity in Content)
                {
                    if (contentIdentity is TrooperIdentity trooper)
                    {
                        AnalyticsManager.Instance.SendOpenNewTrooper(trooper.Class, trooper.Level, trooper.Grade.ToString(), string.Empty, 0);
                    }
                }
            }
            else if (Content.Count() == 1 && Content.First() is TrooperIdentity trooper)
            {
                AnalyticsManager.Instance.SendOpenNewTrooperWithLocalPrice(trooper.Class, trooper.Level, trooper.Grade.ToString(), Price);
            }

            Game.ApplyContent(Content);
            Game.Player.PayPrice(Price);

            base.OnAcquiringSucceeded();
        }
    }
}
