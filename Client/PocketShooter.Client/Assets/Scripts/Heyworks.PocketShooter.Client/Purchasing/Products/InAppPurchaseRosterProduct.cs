using Heyworks.PocketShooter.Meta.Communication;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.Modules.Analytics;
using Heyworks.PocketShooter.Purchasing.AcquiringFlows;
using Heyworks.PocketShooter.Purchasing.PurchaseManager.CommonEntities;

namespace Heyworks.PocketShooter.Purchasing.Products
{
    /// <summary>
    /// This class encapsulates information about concrete in-app purchase, available in the game's shop.
    /// </summary>
    public class InAppPurchaseRosterProduct : RosterProduct
    {
        private readonly IPurchaseManager purchaseManager;

        /// <summary>
        /// Gets localized name of the shop's product.
        /// </summary>
        public override string Name => PurchaseData.Title;

        /// <summary>
        /// Gets price of the shop's product.
        /// </summary>
        public override Price Price => RosterProductData.Price;

        /// <summary>
        /// Gets the product description.
        /// </summary>
        public override string Description => PurchaseData.Description;

        /// <summary>
        /// Gets formatted price.
        /// </summary>
        public string FormattedPrice => PurchaseData.FormatedPrice;

        /// <summary>
        /// Gets the pre-configured information about purchase.
        /// </summary>
        private InAppPurchase InAppPurchase
        {
            get;
        }

        /// <summary>
        /// Gets the information about purchase, necessary for representation.
        /// </summary>
        private PurchaseRepresentationData PurchaseData
        {
            get;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InAppPurchaseRosterProduct"/> class.
        /// </summary>
        public InAppPurchaseRosterProduct(
            IPurchaseManager purchaseManager,
            InAppPurchase inAppPurchase,
            PurchaseRepresentationData purchaseData,
            RosterProductData rosterProduct,
            IGameHubClient shopComponent,
            MetaGame game)
            : base(rosterProduct, shopComponent, game)
        {
            this.purchaseManager = purchaseManager;
            InAppPurchase = inAppPurchase;
            PurchaseData = purchaseData;
        }

        /// <summary>
        /// Gets the acquiring flow of the shop product.
        /// </summary>
        protected override ShopProductAcquiringFlow CreateAcquiringFlow()
        {
            return new InAppPurchaseProductAcquiringFlow(purchaseManager, ShopComponent, InAppPurchase.Id);
        }

        protected override void OnAcquiringSucceeded()
        {
            AnalyticsManager.Instance.SendCharecterInAppPurchase(InAppPurchase.Id, InAppPurchase.PriceUSD, PurchaseData.CurrencySymbol, (double)PurchaseData.LocalPrice, PurchaseData.TransactionID);

            if (RosterProductData.Content is TrooperIdentity trooper)
            {
                AnalyticsManager.Instance.SendOpenNewTrooperWithPriceUSD(trooper.Class, trooper.Level, trooper.Grade.ToString(), InAppPurchase.PriceUSD);
            }
           
            Game.ApplyContent(new[] { RosterProductData.Content });

            base.OnAcquiringSucceeded();
        }
    }
}
