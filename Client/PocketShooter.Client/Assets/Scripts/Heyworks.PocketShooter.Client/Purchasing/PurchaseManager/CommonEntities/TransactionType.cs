namespace Heyworks.PocketShooter.Purchasing.PurchaseManager.CommonEntities
{
    /// <summary>
    /// Represents all in-app transactions types.
    /// </summary>
    public enum TransactionType
    {
        /// <summary>
        /// The confirm of started before transaction.
        /// </summary>
        Confirm = 0,

        /// <summary>
        /// The restore of completed transaction.
        /// </summary>
        Restore = 1,

        /// <summary>
        /// The new acquire of in-app product. 
        /// </summary>
        NewAcquire = 2
    }
}