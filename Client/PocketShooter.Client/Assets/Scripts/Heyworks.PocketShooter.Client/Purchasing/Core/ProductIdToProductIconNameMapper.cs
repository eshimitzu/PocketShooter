using System;

namespace Heyworks.PocketShooter.Purchasing.Core
{
    /// <summary>
    /// Helps to match product ID to icon name of product in store.
    /// </summary>
    public static class ProductIdToProductIconNameMapper
    {
        #region [Product IDs]

        private const string TEST1_ID = "com.heyworks.pocketshooter.test1";

        #endregion

        #region [Product icon names]

        private const string TEST1_ICON = "Test1";

        #endregion

        /// <summary>
        /// Gets the purchase icon.
        /// </summary>
        /// <param name="id">Purchase id.</param>
        public static string GetPurchaseIcon(string id)
        {
            switch (id)
            {
                case TEST1_ID:
                    return TEST1_ICON;
                default:
                    throw new InvalidOperationException($"Purchase icon for id {id} not found. ");
            }
        }
    }
}
