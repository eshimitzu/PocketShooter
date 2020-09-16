using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Google;
using Google.Apis.AndroidPublisher.v3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Http;
using Google.Apis.Services;
using Heyworks.PocketShooter.Meta.Services.Configuration;
using Microsoft.Extensions.Logging;

namespace Heyworks.PocketShooter.Meta.Services.Purchases
{
    /// <summary>
    /// Represents a component which can verify Google in-app purchase via Android publisher API.
    /// </summary>
    internal class GooglePurchaseStatusVerifier : IPurchaseStatusVerifier
    {
        private readonly GoogleReceiptData receipt;
        private readonly GooglePurchaseOptions purchaseOptions;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GooglePurchaseStatusVerifier"/> class with Google Receipt data.
        /// </summary>
        /// <param name="receipt">The Google receipt data.</param>
        /// <param name="logger">The logger.</param>
        public GooglePurchaseStatusVerifier(GoogleReceiptData receipt, GooglePurchaseOptions purchaseOptions, ILogger logger)
        {
            this.receipt = receipt.NotNull();
            this.purchaseOptions = purchaseOptions.NotNull();
            this.logger = logger.NotNull();
        }

        /// <summary>
        /// Performs verifications and returns true if android purchase has passed verification.
        /// </summary>
        public async Task<bool> CheckPurchaseStatusAsync()
        {
            var flow = SetupAuthorizationFlow();
            var userCredentials = await GetUserCredentials(flow);

            using (var service = new AndroidPublisherService(new BaseClientService.Initializer
            {
                HttpClientInitializer = userCredentials,
            }))
            {
                try
                {
                    var purchase = await service.Purchases.Products.Get(receipt.PackageName, receipt.ProductId, receipt.PurchaseToken).ExecuteAsync();
                    if (purchase != null)
                    {
                        logger.LogDebug(
                            "Received purchase information from Android publisher API. Order id: {orderId}, product id: {productId}, purchase state: {purchaseState}, consumption state: {consumptionState}",
                            receipt.OrderId,
                            receipt.ProductId,
                            purchase.PurchaseState,
                            purchase.ConsumptionState);
                    }
                    else
                    {
                        logger.LogDebug(
                            "Android publisher API returns empty purchase information. Order id: {orderId}, product id: {productId}",
                            receipt.OrderId,
                            receipt.ProductId);
                    }

                    // Purchase data should not be empty. Purchase state should be 'purchased', consumption state - 'not consumed'.
                    return purchase != null
                        && (purchase.PurchaseState.HasValue
                        && (GooglePurchaseState)purchase.PurchaseState == GooglePurchaseState.Purchased
                        && purchase.ConsumptionState.HasValue
                        && (AndroidPurchaseConsumptionState)purchase.ConsumptionState == AndroidPurchaseConsumptionState.YetToBeConsumed);
                }
                catch (GoogleApiException ex)
                {
                    if (ex.HttpStatusCode == HttpStatusCode.NotFound)
                    {
                        logger.LogWarning(
                            ex,
                            "Google In-App Purchase with token {purchaseToken}, order id {orderId} for product {productId} was not found.",
                            receipt.PurchaseToken,
                            receipt.OrderId,
                            receipt.ProductId);
                    }
                    else
                    {
                        logger.LogWarning(
                            ex,
                            "Google In-App Purchase verification was failed for order id: {orderId}, product id: {productId}.",
                            receipt.OrderId,
                            receipt.ProductId);
                    }

                    return false;
                }
            }
        }

        private AuthorizationCodeFlow SetupAuthorizationFlow()
        {
            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = purchaseOptions.ApiClientId,
                    ClientSecret = purchaseOptions.ApiClientSecret,
                },

                Scopes = new[] { AndroidPublisherService.Scope.Androidpublisher },
                HttpClientFactory = new HttpClientFactory(),
            });

            return flow;
        }

        private async Task<UserCredential> GetUserCredentials(AuthorizationCodeFlow flow)
        {
            var user = purchaseOptions.ApiUserId;
            var refreshToken = purchaseOptions.ApiRefreshToken;

            var token = await flow.RefreshTokenAsync(user, refreshToken, CancellationToken.None);

            return new UserCredential(flow, user, token);
        }
    }
}
