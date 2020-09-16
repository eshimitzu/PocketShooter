using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.Meta.Services;
using Heyworks.PocketShooter.Meta.Services.Configuration;
using Heyworks.PocketShooter.Meta.Services.Purchases;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Orleans;
using Orleans.Concurrency;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public class ShopGrain : Grain, IShopGrain
    {
        private readonly IGameFactory gameFactory;
        private readonly IConfigurationsProvider configurationsProvider;
        private readonly IMongoCollection<PaymentTransaction> paymentTransactions;
        private readonly PurchaseOptions purchaseOptions;
        private readonly IHostingEnvironment environment;
        private readonly ILoggerFactory loggerFactory;
        private readonly ILogger logger;

        public ShopGrain(
            IGameFactory gameFactory,
            IConfigurationsProvider configurationsProvider,
            IMongoCollection<PaymentTransaction> paymentTransactions,
            IOptions<PurchaseOptions> purchaseOptions,
            IHostingEnvironment environment,
            ILoggerFactory loggerFactory)
        {
            this.gameFactory = gameFactory;
            this.configurationsProvider = configurationsProvider;
            this.paymentTransactions = paymentTransactions;
            this.purchaseOptions = purchaseOptions.Value;
            this.environment = environment;
            this.loggerFactory = loggerFactory;
            this.logger = loggerFactory.CreateLogger<ShopGrain>();
        }

        /// <summary>
        /// Makes a purchase.
        /// </summary>
        /// <param name="receiptData">The purchase receipt data.</param>
        /// <exception cref="InvalidPaymentReceiptException">When the payment receipt is invalid.</exception>
        /// <exception cref="TransactionExistsException">When the payment transaction is already exists in the system.</exception>
        public async Task MakePurchase(string receiptData)
        {
            var game = await gameFactory.Create(this.GetPrimaryKey());
            var player = game.Player;

            var shopConfiguration = await configurationsProvider.GetShopConfiguration(player.Group);
            PaymentReceipt paymentReceipt = CreatePaymentReceipt(receiptData, player.ApplicationStore, shopConfiguration);

            try
            {
                PaymentTransaction paymentTransaction = await paymentReceipt.VerifyAsync();
                paymentTransaction.PlayerId = player.Id;

                if (!paymentTransaction.IsSandbox)
                {
                    paymentTransactions.InsertOne(paymentTransaction);
                }

                var purchase = game.Shop.GetPurchase(paymentTransaction.ProductId);
                var playerBalanceGrain = GrainFactory.GetGrain<IPlayerBalanceGrain>(this.GetPrimaryKey());
                await playerBalanceGrain.RegisterPurchase(purchase.AsImmutable());

                var purchaseContent = game.Shop.GetPurchaseContent(paymentTransaction.ProductId);
                var gameContentGrain = GrainFactory.GetGrain<IGameContentGrain>(this.GetPrimaryKey());
                await gameContentGrain.ApplyContent(purchaseContent.AsImmutable());
            }
            catch (InvalidPaymentReceiptException ex)
            {
                logger.LogWarning(ex, "Can't purchase product. Player: {player}, Payment receipt: {paymentReceipt}", player, paymentReceipt);
                throw;
            }
            catch (TransactionExistsException ex)
            {
                logger.LogWarning(ex, "Can't purchase product. Player: {player}, Payment receipt: {paymentReceipt}", player, paymentReceipt);
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error has occurred while verifying payment receipt. Player: {player}, Payment receipt: {paymentReceipt}", player, paymentReceipt);
                throw;
            }
        }

        /// <summary>
        /// Buys a roster product with specified id. Returns <c>true</c> if succeeds; otherwise returns <c>false</c>.
        /// </summary>
        /// <param name="productId">The roster product id.</param>
        /// <returns><c>true</c> if succeeds; otherwise returns <c>false</c>.</returns>
        public async Task<bool> BuyRosterProduct(string productId)
        {
            var game = await gameFactory.Create(this.GetPrimaryKey());

            if (!game.CanBuyRosterProduct(productId))
            {
                return false;
            }

            var rosterProduct = game.Shop.GetRosterProduct(productId);

            var playerBalanceGrain = GrainFactory.GetGrain<IPlayerBalanceGrain>(this.GetPrimaryKey());
            await playerBalanceGrain.PayPrice(rosterProduct.Price.AsImmutable());

            var gameContentGrain = GrainFactory.GetGrain<IGameContentGrain>(this.GetPrimaryKey());
            IEnumerable<IContentIdentity> content = new[] { rosterProduct.Content };
            await gameContentGrain.ApplyContent(content.AsImmutable());

            return true;
        }

        /// <summary>
        /// Buys a shop product with specified id. Returns <c>true</c> if succeeds; otherwise returns <c>false</c>.
        /// </summary>
        /// <param name="productId">The shop product id.</param>
        /// <returns><c>true</c> if succeeds; otherwise returns <c>false</c>.</returns>
        public async Task<bool> BuyShopProduct(string productId)
        {
            var game = await gameFactory.Create(this.GetPrimaryKey());

            if (!game.CanBuyShopProduct(productId))
            {
                return false;
            }

            var shopProduct = game.Shop.GetShopProduct(productId);

            var playerBalanceGrain = GrainFactory.GetGrain<IPlayerBalanceGrain>(this.GetPrimaryKey());
            await playerBalanceGrain.PayPrice(shopProduct.Price.AsImmutable());

            var gameContentGrain = GrainFactory.GetGrain<IGameContentGrain>(this.GetPrimaryKey());
            await gameContentGrain.ApplyContent(shopProduct.Content.AsImmutable());

            return true;
        }

        private PaymentReceipt CreatePaymentReceipt(string receiptData, ApplicationStoreName storeName, IShopConfigurationBase shopConfiguration)
        {
            if (environment.IsLocalOrDevelopment() ||
                (storeName == ApplicationStoreName.Apple && environment.IsTesting()))
            {
                var purchase = shopConfiguration.GetPurchase(receiptData);

                return new TestPaymentReceipt(purchase.Id);
            }
            else
            {
                switch (storeName)
                {
                    case ApplicationStoreName.Google:
                        return new GooglePaymentReceipt(
                            receiptData,
                            paymentTransactions,
                            shopConfiguration,
                            purchaseOptions.Google,
                            loggerFactory);
                    default:
                        throw new NotImplementedException($"The given application store '{storeName}' is not supported.");
                }
            }
        }
    }
}
