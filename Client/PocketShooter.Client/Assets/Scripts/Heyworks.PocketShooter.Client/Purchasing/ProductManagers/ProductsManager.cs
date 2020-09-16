using System;
using System.Collections.Generic;
using Heyworks.PocketShooter.Purchasing.Products;

namespace Heyworks.PocketShooter.Purchasing.ProductManagers
{
    /// <summary>
    /// This class provides functionality to get collection of <see cref="Product"/> objects.
    /// </summary>
    public abstract class ProductsManager
    {
        /// <summary>
        /// Event fires when acquiring of some product has just started.
        /// </summary>
        public event Action ProductAcquiringStarted;

        /// <summary>
        /// Event fires when some product acquiring has just been finished either successfully or not.
        /// </summary>
        public event Action ProductAcquiringFinished;

        /// <summary>
        /// Event fires, when the collection of products, which can be created is changed.
        /// </summary>
        public event Action ProductsUpdated;

        /// <summary>
        /// Manually runs the process of updating of list of instances of <see cref="Product"/> class,
        /// which can be received from the manager.
        /// </summary>
        protected virtual void Update()
        {
        }

        /// <summary>
        /// Get the list of <see cref="Product"/> objects.
        /// </summary>
        public abstract IEnumerable<Product> GetProducts(Predicate<Product> match = null);

        /// <summary>
        /// Fires the event, notifying about that the acquiring of a product has just started.
        /// </summary>
        protected void OnProductAcquiringStarted()
        {
            var handler = ProductAcquiringStarted;
            if (handler != null)
            {
                handler();
            }
        }

        /// <summary>
        /// Fires the event, notifying about that the acquiring of a product has just finished.
        /// </summary>
        protected void OnProductAcquiringFinished()
        {
            var handler = ProductAcquiringFinished;
            if (handler != null)
            {
                handler();
            }
        }

        /// <summary>
        /// Fires the event, notifying about that the list of products has just been updated.
        /// </summary>
        protected void OnProductsUpdated()
        {
            var handler = ProductsUpdated;
            if (handler != null)
            {
                handler();
            }
        }
    }
}
