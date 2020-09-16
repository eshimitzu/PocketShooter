using Heyworks.PocketShooter.Meta.Communication;

namespace Heyworks.PocketShooter.Purchasing.AcquiringFlows
{
    public class InGameRosterProductAcquiringFlow : ShopProductAcquiringFlow
    {
        private readonly IGameHubClient shopComponent;
        private readonly string productId;

        /// <summary>
        /// Initializes a new instance of the <see cref="InGameRosterProductAcquiringFlow"/> class.
        /// </summary>
        /// <param name="shopComponent"> Object providing access to the server operations connected with shop. </param>
        /// <param name="productId"> Id of the roster product. </param>
        public InGameRosterProductAcquiringFlow(
            IGameHubClient shopComponent,
            string productId)
        {
            this.shopComponent = shopComponent;
            this.productId = productId;
        }

        /// <summary>
        /// Starts the flow of product acquiring.
        /// </summary>
        public override void StartAcquiring()
        {
            BuyProductAsync();
        }

        private async void BuyProductAsync()
        {
            await shopComponent.BuyRosterProductAsync(productId);
            OnAcquireSucceeded();
        }
    }
}
