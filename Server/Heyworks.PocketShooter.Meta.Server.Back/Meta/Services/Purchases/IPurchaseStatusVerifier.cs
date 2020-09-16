using System.Threading.Tasks;

namespace Heyworks.PocketShooter.Meta.Services.Purchases
{
    /// <summary>
    /// Describes a component which can verify device in-app purchase via remote platform-specific verification API.
    /// </summary>
    internal interface IPurchaseStatusVerifier
    {
        /// <summary>
        /// Performs verifications and returns true if purchase has passed verification.
        /// </summary>
        Task<bool> CheckPurchaseStatusAsync();
    }
}
