using System;

namespace Heyworks.PocketShooter.Purchasing.AcquiringFlows
{
    /// <summary>
    /// Represents an object which encapsulates the flow of a concrete shop product acquiring.
    /// </summary>
    public abstract class ShopProductAcquiringFlow
    {
        /// <summary>
        /// Rises when the encapsulated acquirement is succeeded.
        /// </summary>
        public event Action AcquireSucceeded;

        /// <summary>
        /// Rises when the encapsulated acquirement is failed.
        /// </summary>
        public event Action AcquireFailed;

        /// <summary>
        /// Starts the flow of product acquiring.
        /// </summary>
        public abstract void StartAcquiring();

        /// <summary>
        /// Rises the event notifying about successful acquirement.
        /// </summary>
        protected void OnAcquireSucceeded()
        {
            var handler = AcquireSucceeded;
            if (handler != null)
            {
                handler();
            }
        }

        /// <summary>
        /// Rises the event notifying about failure of acquirement.
        /// </summary>
        protected void OnAcquireFailed()
        {
            var handler = AcquireFailed;
            if (handler != null)
            {
                handler();
            }
        }
    }
}
