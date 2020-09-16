using System;
using Heyworks.PocketShooter.Meta.Communication;
using Heyworks.PocketShooter.Purchasing.PurchaseManager.CommonEntities;
using Microsoft.Extensions.Logging;

namespace Heyworks.PocketShooter.Purchasing.Products
{
    /// <summary>
    /// Encapsulates flow of confirmation of in-app purchase through PT server.
    /// </summary>
    public sealed class PurchaseConfirmationFlow
    {
        #region [Private fields]

        private readonly IGameHubClient shopComponent;
        private readonly IPurchaseManager purchaseManager;

        private PurchaseReceipt purchaseReceipt;

        #endregion

        #region [Events]

        /// <summary>
        /// Event fires if purchase has been successfully confirmed and consumed.
        /// </summary>
        public event Action ConfirmationSucceeded;

        /// <summary>
        /// Event fires if purchase hasn't been confirmed.
        /// </summary>
        public event Action ConfirmationFailed;

        #endregion

        #region [Constructor]

        /// <summary>
        /// Initializes a new instance of the <see cref="PurchaseConfirmationFlow"/> class.
        /// </summary>
        /// <param name="purchaseManager"> Object, providing interface to make in-app purchase. </param>
        /// <param name="shopComponent"> Object, encapsulating most server operations connected with shop. </param>
        public PurchaseConfirmationFlow(
            IPurchaseManager purchaseManager,
            IGameHubClient shopComponent)
        {
            this.shopComponent = shopComponent;
            this.purchaseManager = purchaseManager;
        }

        #endregion

        #region [Public methods]

        /// <summary>
        /// Confirm the purchase according to the specified receipt.
        /// </summary>
        /// <param name="receipt"> Receipt for the purchase, which is to be confirmed. </param>
        public void ConfirmPurchase(PurchaseReceipt receipt)
        {
            purchaseReceipt = receipt;

            AddPurchaseManagerEventHandlers();

            SendConfirmationRequest(purchaseReceipt.ReceiptData);
        }

        #endregion

        #region [Private methods]

        private async void SendConfirmationRequest(string data)
        {
            PurchaseLog.Log.LogTrace("Send confirmation request to validate transaction with id: {0}", data);

            var operation = await shopComponent.MakePurchaseAsync(data);

            if (operation.IsOk)
            {
                ConfirmTransactionOperation_Succeeded();
            }
            else
            {
                HandleErrorResponse(operation);
            }
        }

        private void HandleErrorResponse(ResponseOption operation)
        {
            if (operation.IsError)
            {
                switch (operation.Error.Code)
                {
                    case ApiErrorCode.PaymentTransactionExists:
                        ConfirmTransactionOperation_PaymentTransactionExistsError();
                        break;
                    case ApiErrorCode.InvalidPaymentReceipt:
                        ConfirmTransactionOperation_InvalidPaymentReceiptError();
                        break;
                    default:
                        HandleConfirmationFailure(false);
                        break;
                }
            }
            else
            {
                HandleConfirmationFailure(false);
            }
        }

        private void AddPurchaseManagerEventHandlers()
        {
            purchaseManager.PurchaseTransactionFailed += PurchaseManager_PurchaseTransactionFailed;
            purchaseManager.PurchaseTransactionClosed += PurchaseManagerPurchase_PurchaseTransactionClosed;
            purchaseManager.PurchaseTransactionCanceled += PurchaseManager_PurchaseTransactionCanceled;
        }

        private void RemovePurchaseManagerEventHandlers()
        {
            purchaseManager.PurchaseTransactionFailed -= PurchaseManager_PurchaseTransactionFailed;
            purchaseManager.PurchaseTransactionClosed -= PurchaseManagerPurchase_PurchaseTransactionClosed;
            purchaseManager.PurchaseTransactionCanceled -= PurchaseManager_PurchaseTransactionCanceled;
        }

        private void HandleConfirmationFailure(bool isPurchaseConsumed)
        {
            if (isPurchaseConsumed)
            {
                purchaseManager.CloseCurrentPurchaseTransaction();
            }
            else
            {
                purchaseManager.PostponeCurrentPurchaseTransaction();
            }

            RemovePurchaseManagerEventHandlers();
            OnConfirmationFailed();
        }

        #endregion

        #region [Static event handlers]

        private void PurchaseManager_PurchaseTransactionFailed(string message)
        {

            PurchaseLog.Log.LogWarning("Shop. Purchase has FAILED. Message: {0};", message);

            HandleConfirmationFailure(false);
        }

        private void PurchaseManagerPurchase_PurchaseTransactionClosed()
        {
            PurchaseLog.Log.LogTrace("Shop. Last purchase has SUCCEEDED.");

            RemovePurchaseManagerEventHandlers();
            OnConfirmationSucceeded();
        }

        private void PurchaseManager_PurchaseTransactionCanceled(string message)
        {
            PurchaseLog.Log.LogWarning("Shop. Purchase has been CANCELED. Message: {0};", message);

            HandleConfirmationFailure(false);
        }

        #endregion

        #region [Operation event handlers]

        private void ConfirmTransactionOperation_Succeeded()
        {
            PurchaseLog.Log.LogTrace("Confirmation of transaction has succeeded.");

            purchaseManager.CloseCurrentPurchaseTransaction();
        }

        private void ConfirmTransactionOperation_PaymentTransactionExistsError()
        {
            PurchaseLog.Log.LogWarning("Confirmation of transaction has failed. Payment transaction exist.");

            HandleConfirmationFailure(true);
        }

        private void ConfirmTransactionOperation_InvalidPaymentReceiptError()
        {
            PurchaseLog.Log.LogWarning("Confirmation of transaction has failed. Invalid payment receipt.");

            HandleConfirmationFailure(false);
        }

        #endregion

        #region [Event invokators]

        private void OnConfirmationSucceeded()
        {
            var handler = ConfirmationSucceeded;
            if (handler != null)
            {
                handler();
            }
        }

        private void OnConfirmationFailed()
        {
            var handler = ConfirmationFailed;
            if (handler != null)
            {
                handler();
            }
        }

        #endregion
    }
}
