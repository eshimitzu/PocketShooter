namespace Heyworks.PocketShooter.Meta.Communication
{
    /// <summary>
    /// Enumeration contains error codes which can be returned by API.
    /// </summary>
    public enum ApiErrorCode
    {
        /// <summary>
        /// Returned in case when user for specified parameters was not found in DB.
        /// </summary>
        UserNotFound = 50,

        /// <summary>
        /// Returned in case when player's match results is not found.
        /// </summary>
        MatchResultsNotFound = 51,

        /// <summary>
        /// Returned in case when register request data provided was invalid and the server
        /// was not able to deserialize it or some of the fields were empty.
        /// </summary>
        InvalidRegisterRequest = 70,

        /// <summary>
        /// Returned in case when login request data provided was invalid and the server
        /// was not able to deserialize it or some of the fields were empty.
        /// </summary>
        InvalidLoginRequest = 71,

        /// <summary>
        /// Returned in case if social credentials from client are invalid.
        /// </summary>
        InvalidSocialCredentials = 72,

        /// <summary>
        /// Returned in case when <see cref="SocialConnectRequest"/> data is invalid.
        /// </summary>
        InvalidSocialConnectRequest = 73,

        /// <summary>
        /// Returned in case when user tried to connect via non-existent social account but the profile is already connected to another social account.
        /// </summary>
        UserAlreadyConnected = 74,

        /// <summary>
        /// Returned in case Social connect operation needs to switch users.
        /// </summary>
        SocialAccountConnectedToAnotherUser = 75,

        /// <summary>
        /// The client version is not supported.
        /// </summary>
        InvalidClientVersion = 105,

        /// <summary>
        /// Returned in case when register request comes to server with the device ID that is already registered.
        /// </summary>
        DeviceAlreadyExist = 110,

        /// <summary>
        /// Returned in case when a change nickname operation can not be performed.
        /// </summary>
        InvalidNickname = 150,

        /// <summary>
        /// Returned in case when the payment receipt is invalid.
        /// </summary>
        InvalidPaymentReceipt = 170,

        /// <summary>
        /// Returned in case when the payment transaction is already exists.
        /// </summary>
        PaymentTransactionExists = 171,

        /// <summary>
        /// Client internal code. Returned in case when social network token was not received.
        /// </summary>
        SocialNetworkAccessTokenReceiveFailed = 200,

        /// <summary>
        /// Returned in case of internal server error.
        /// </summary>
        InternalServerError = 500
    }
}
