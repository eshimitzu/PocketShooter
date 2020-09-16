using System;
using System.Collections.Generic;
using Heyworks.PocketShooter.Meta.Communication;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Meta.Entities;

namespace Heyworks.PocketShooter.Purchasing.Products
{
    public abstract class ShopProduct : Product
    {
        private readonly ShopProductData shopProductData;

        public IEnumerable<IContentIdentity> Content => shopProductData.Content;

        public IReadOnlyList<ShopCategory> Category => shopProductData.Category;

        protected MetaGame Game { get; }

        protected IGameHubClient ShopComponent { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShopProduct"/> class.
        /// </summary>
        protected ShopProduct(
            ShopProductData shopProductData,
            IGameHubClient shopComponent,
            MetaGame game)
            : base(shopProductData.Id)
        {
            this.shopProductData = shopProductData;
            ShopComponent = shopComponent;
            Game = game;
        }

        /// <summary>
        /// Gets price of the shop's product.
        /// </summary>
        public override Price Price => shopProductData.Price;

        /// <summary>
        /// Gets localized description of the shop's product.
        /// </summary>
        public override string Description => throw new NotImplementedException();

        /// <summary>
        /// Tells if the shop's product, represented by the  object, can be acquired.
        /// </summary>
        public override bool CanAcquire()
        {
            return Game.Player.CanPayPrice(shopProductData.Price) && IsVisible;
        }

        public bool IsLocked => Game.Player.Level < shopProductData.MinPlayerLevel ||
                                Game.Player.Level > shopProductData.MaxPlayerLevel;

        public bool IsVisible => !IsLocked && !HasAnyPurchasedContent;

        private bool HasAnyPurchasedContent => Game.Army.HasAnyContent(Content);
    }
}
