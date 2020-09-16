using System;

namespace Heyworks.PocketShooter.Purchasing.Products
{
    /// <summary>
    /// Provides functionality to track the flow of shop product acquiring.
    /// </summary>
    public interface IProductAcquiring
    {
        /// <summary>
        /// Event fires when acquiring of the shop product is just started.
        /// </summary>
        event Action<IProductAcquiring> AcquiringStarted;

        /// <summary>
        /// Event fires when shop's product, represented by the object, has been successfully acquired.
        /// </summary>
        event Action<IProductAcquiring> AcquireSucceeded;

        /// <summary>
        /// Event fires when shop's product, represented by the object, has failed to be acquired.
        /// </summary>
        event Action<IProductAcquiring> AcquireFailed;

        /// <summary>
        /// Tells if the shop's product, represented by the  object, can be acquired.
        /// </summary>
        bool CanAcquire();

        /// <summary>
        /// Start the process of the shop's product acquiring.
        /// </summary>
        void Acquire();
    }
}
