using System;
using Heyworks.PocketShooter.Meta.Communication;
using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.UI;

namespace Heyworks.PocketShooter.Purchasing.Products
{
    public abstract class RosterProduct : Product
    {
        /// <summary>
        /// Gets product content.
        /// </summary>
        public IContentIdentity Content => RosterProductData.Content;

        public bool IsLocked => Game.Player.Level < PlayerLevelForUnlock;

        public int PlayerLevelForUnlock => RosterProductData.PlayerLevelForUnlock;

        /// <summary>
        /// Gets price of the shop's product.
        /// </summary>
        public override Price Price => RosterProductData.Price;

        /// <summary>
        /// Gets localized name of the shop's product.
        /// </summary>
        public override string Name => RosterProductData.Id.Localized();

        /// <summary>
        /// Gets localized description of the shop's product.
        /// </summary>
        public override string Description => throw new NotImplementedException();

        public bool IsPurchased => Game.Army.HasContent(Content);

        protected RosterProductData RosterProductData { get; }

        protected MetaGame Game { get; }

        protected IGameHubClient ShopComponent { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RosterProduct"/> class.
        /// </summary>
        protected RosterProduct(
            RosterProductData rosterProduct,
            IGameHubClient shopComponent,
            MetaGame game)
            : base(rosterProduct.Id)
        {
            RosterProductData = rosterProduct;
            ShopComponent = shopComponent;
            Game = game;
        }

        /// <summary>
        /// Tells if the shop's product, represented by the  object, can be acquired.
        /// </summary>
        public override bool CanAcquire()
        {
            return !IsPurchased && !IsLocked && Game.Player.CanPayPrice(Price);
        }
    }
}
