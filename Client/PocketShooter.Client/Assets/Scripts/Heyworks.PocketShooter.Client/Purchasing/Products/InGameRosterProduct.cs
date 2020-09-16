using Heyworks.PocketShooter.Meta.Communication;
using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.Modules.Analytics;
using Heyworks.PocketShooter.Purchasing.AcquiringFlows;

namespace Heyworks.PocketShooter.Purchasing.Products
{
    public class InGameRosterProduct : RosterProduct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InGameRosterProduct"/> class.
        /// </summary>
        public InGameRosterProduct(RosterProductData rosterProduct, IGameHubClient shopComponent, MetaGame game)
            : base(rosterProduct, shopComponent, game)
        {
        }

        /// <summary>
        /// Gets the acquiring flow of the shop product.
        /// </summary>
        protected override ShopProductAcquiringFlow CreateAcquiringFlow()
        {
            return new InGameRosterProductAcquiringFlow(ShopComponent, RosterProductData.Id);
        }

        protected override void OnAcquiringSucceeded()
        {
            if (RosterProductData.Content is TrooperIdentity trooper)
            {
                AnalyticsManager.Instance.SendOpenNewTrooperWithLocalPrice(trooper.Class, trooper.Level, trooper.Grade.ToString(), RosterProductData.Price);
            }

            Game.ApplyContent(new[] { RosterProductData.Content });
            Game.Player.PayPrice(RosterProductData.Price);

            base.OnAcquiringSucceeded();
        }
    }
}
