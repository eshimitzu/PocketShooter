using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.Meta.Services.Configuration;
using Heyworks.PocketShooter.Meta.Services.Cryptography;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Heyworks.PocketShooter.Meta.Services.Purchases
{
    /// <summary>
    /// Represents Google payment receipt.
    /// </summary>
    internal class GooglePaymentReceipt : PaymentReceipt
    {
        private readonly GooglePurchaseOptions purchaseOptions;
        private readonly ILoggerFactory loggerFactory;
        private readonly ILogger logger;

        private readonly string receiptDataString;

        private readonly IMongoCollection<PaymentTransaction> paymentTransactions;
        private readonly IShopConfigurationBase shopConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="GooglePaymentReceipt"/> class with receipt data, signature, transactions repository and player's configuration.
        /// </summary>
        /// <param name="receiptDataString">The string containing receipt data.</param>
        /// <param name="paymentTransactions">The payment transactions collection.</param>
        /// <param name="shopConfiguration">The shop configuration.</param>
        /// <param name="purchaseOptions">The purchase options.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public GooglePaymentReceipt(
            string receiptDataString,
            IMongoCollection<PaymentTransaction> paymentTransactions,
            IShopConfigurationBase shopConfiguration,
            GooglePurchaseOptions purchaseOptions,
            ILoggerFactory loggerFactory)
        {
            this.receiptDataString = receiptDataString.NotNull();
            this.paymentTransactions = paymentTransactions.NotNull();
            this.shopConfiguration = shopConfiguration.NotNull();
            this.purchaseOptions = purchaseOptions.NotNull();
            this.loggerFactory = loggerFactory.NotNull();

            this.logger = loggerFactory.CreateLogger<GooglePaymentReceipt>();
        }

        /// <summary>
        /// Verifies the current receipt and returns the payment transaction if receipt is valid.
        /// Throws an exception if receipt is invalid.
        /// </summary>
        /// <exception cref="InvalidPaymentReceiptException">When the payment receipt is invalid.</exception>
        /// <exception cref="TransactionExistsException">When the payment transaction is already exists in the system.</exception>
        public override async Task<PaymentTransaction> VerifyAsync()
        {
            GoogleReceiptData receiptData;
            string receiptDataJson;
            string receiptDataSignature;

            // Try to deserialize receiptData into GoogleReceiptData instance.
            if (!TryGetReceipt(out receiptData, out receiptDataJson, out receiptDataSignature))
            {
                throw new InvalidPaymentReceiptException("The payment receipt structure is invalid.");
            }

            // Verify receipt signature.
            if (!VerifySignature(receiptData, receiptDataJson, receiptDataSignature))
            {
                throw new InvalidPaymentReceiptException("The purchase data signature is invalid.");
            }

            // verify that receipt is for our application.
            if (!VerifyReceiptProduct(receiptData))
            {
                throw new InvalidPaymentReceiptException("The payment receipt product is invalid.");
            }

            if (!receiptData.IsSandbox)
            {
                // Verify that receipt transaction does not exist in the database.
                if (!await VerifyReceiptTransaction(receiptData))
                {
                    throw new TransactionExistsException($"The payment transaction with id {receiptData.OrderId} is already exists.");
                }
            }

            var purchaseStatusVerifier = new GooglePurchaseStatusVerifier(
                receiptData,
                purchaseOptions,
                loggerFactory.CreateLogger<GooglePurchaseStatusVerifier>());

            // Verify purchase status at Google servers.
            bool isPurchaseStatusValid = await purchaseStatusVerifier.CheckPurchaseStatusAsync();
            if (!isPurchaseStatusValid)
            {
                throw new InvalidPaymentReceiptException("The Google purchase status verification failed.");
            }

            return new GooglePaymentTransaction()
            {
                TransactionId = receiptData.OrderId,
                ProductId = receiptData.ProductId,
                PackageName = receiptData.PackageName,
                PurchaseDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(receiptData.PurchaseTime),
                PurchaseToken = receiptData.PurchaseToken,
                DeveloperPayload = receiptData.DeveloperPayload,
            };
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents the current <see cref="GooglePaymentReceipt"/>.
        /// </summary>
        public override string ToString()
        {
            return $@"""Type: ""{nameof(GooglePaymentReceipt)}"", Data: ""{receiptDataString}""""";
        }

        /// <summary>
        /// Trying to get a receipt from receipt data.
        /// </summary>
        /// <param name="receiptData">The <see cref="GoogleReceiptData"/> class.</param>
        /// <param name="receiptDataJson">The receipt data JSON.</param>
        /// <param name="receiptDataSignature">The receipt data signature.</param>
        private bool TryGetReceipt(out GoogleReceiptData receiptData, out string receiptDataJson, out string receiptDataSignature)
        {
            try
            {
                dynamic rowData = JsonConvert.DeserializeObject(receiptDataString);
                dynamic payloadData = JsonConvert.DeserializeObject((string)rowData.Payload);

                receiptDataJson = ((JObject)JsonConvert.DeserializeObject((string)payloadData.json)).ToString(Formatting.None);
                receiptDataSignature = (string)payloadData.signature;

                receiptData = JsonConvert.DeserializeObject<GoogleReceiptData>(receiptDataJson);

                return
                    receiptData != null &&
                    !string.IsNullOrWhiteSpace(receiptDataSignature);
            }
            catch (JsonReaderException ex)
            {
                receiptData = null;
                receiptDataJson = null;
                receiptDataSignature = null;
                logger.LogWarning(ex, "Invalid receipt data format. Receipt data: {receiptDataString}", receiptDataString);
            }
            catch (JsonSerializationException ex)
            {
                receiptData = null;
                receiptDataJson = null;
                receiptDataSignature = null;
                logger.LogWarning(ex, "Invalid receipt data format. Receipt data: {receiptDataString}", receiptDataString);
            }

            return false;
        }

        /// <summary>
        /// Verifies a receipt's product.
        /// </summary>
        /// <param name="receiptData">The receipt data to verify.</param>
        private bool VerifyReceiptProduct(GoogleReceiptData receiptData) =>
            shopConfiguration.IsPurchaseExists(receiptData.ProductId);

        /// <summary>
        /// Verifies a receipt signature.
        /// </summary>
        /// <param name="receiptData">The receipt data object.</param>
        /// <param name="receiptDataJson">The receipt data JSON.</param>
        /// <param name="receiptDataSignature">The receipt data signature.</param>
        private bool VerifySignature(GoogleReceiptData receiptData, string receiptDataJson, string receiptDataSignature)
        {
            try
            {
                //// Get bundle id from receipt data here.
                //// Find modulus and exponent using this bundle id and verify signature against these keys.

                string modulus = purchaseOptions.PublicKeyModulus;
                string exponent = purchaseOptions.PublicKeyExponent;

                var rsaHelper = new RSAHelper(new RSAParameters()
                {
                    Modulus = Convert.FromBase64String(modulus),
                    Exponent = Convert.FromBase64String(exponent),
                });

                bool verificationResult = rsaHelper.Verify(receiptDataJson, receiptDataSignature);

                if (!verificationResult)
                {
                    logger.LogWarning(
                        "Failed to verify signature against Google purchase receipt data. Public key (modulus/exponent): '{modulus}' / '{exponent}', signature: '{receiptDataSignature}', receipt data: {receiptDataJson}",
                        modulus,
                        exponent,
                        receiptDataSignature,
                        receiptDataJson);
                }

                return verificationResult;
            }
            catch (CryptographicException ex)
            {
                logger.LogWarning(ex, "Error occurred while verifying receipt signature. Receipt data: {receiptDataJson}", receiptDataJson);
            }

            return false;
        }

        private async Task<bool> VerifyReceiptTransaction(GoogleReceiptData receipt)
        {
            var existingTransaction = await paymentTransactions.Find(item => item.TransactionId == receipt.OrderId).FirstOrDefaultAsync();

            return existingTransaction == null;
        }
    }
}
