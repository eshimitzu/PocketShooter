namespace Heyworks.PocketShooter.Purchasing.PurchaseManager.CommonEntities
{
    /// <summary>
    /// Represents values for showing market transaction state.
    /// </summary>
    public enum MarketTransactionState
    {
        /// <summary>
        /// Request sent state.
        /// </summary>
        RequestSent = 0,

        /// <summary>
        /// Received successful response.
        /// </summary>
        Successful = 1,

        /// <summary>
        /// Received failed response.
        /// </summary>
        Failed = 2,
    }
}
