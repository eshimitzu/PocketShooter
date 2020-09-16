using System;
using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.Purchasing.AcquiringFlows;

namespace Heyworks.PocketShooter.Purchasing.Products
{
    /// <summary>
    /// This class exposes the basic interface for any product, which can be acquired in the game's shop.
    /// </summary>
    public abstract class Product : IProductAcquiring
    {
        #region [Events]

        private event Action<IProductAcquiring> AcquiringStarted;

        private event Action<IProductAcquiring> AcquireSucceeded;

        private event Action<IProductAcquiring> AcquireFailed;

        /// <summary>
        /// Event fires when acquiring of the shop product is just started.
        /// </summary>
        event Action<IProductAcquiring> IProductAcquiring.AcquiringStarted
        {
            add => AcquiringStarted += value;
            remove => AcquiringStarted -= value;
        }

        /// <summary>
        /// Event fires when shop's product, represented by the object, has been successfully acquired.
        /// </summary>
        event Action<IProductAcquiring> IProductAcquiring.AcquireSucceeded
        {
            add => AcquireSucceeded += value;
            remove => AcquireSucceeded -= value;
        }

        /// <summary>
        /// Event fires when shop's product, represented by the object, has failed to be acquired.
        /// </summary>
        event Action<IProductAcquiring> IProductAcquiring.AcquireFailed
        {
            add => AcquireFailed += value;
            remove => AcquireFailed -= value;
        }

        #endregion

        #region [Properties]

        /// <summary>
        /// Gets product identifier.
        /// </summary>
        public string Id
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets price of the shop's product.
        /// </summary>
        public abstract Price Price
        {
            get;
        }

        /// <summary>
        /// Gets localized name of the shop's product.
        /// </summary>
        public abstract string Name
        {
            get;
        }

        /// <summary>
        /// Gets localized description of the shop's product.
        /// </summary>
        public abstract string Description
        {
            get;
        }

        #endregion

        #region [Constructor]

        /// <summary>
        /// Initializes a new instance of the <see cref="Product"/> class.
        /// </summary>
        /// <param name="productId"> Identity of the product.</param>
        protected Product(string productId)
        {
            Id = productId;
        }

        #endregion

        #region [Public methods]

        /// <summary>
        /// Tells if the shop's product, represented by the  object, can be acquired.
        /// </summary>
        public abstract bool CanAcquire();

        /// <summary>
        /// Start the process of the shop's product acquiring.
        /// </summary>
        public void Acquire()
        {
            AcquiringStarted?.Invoke(this);

            var acquiringFlow = CreateAcquiringFlow();
            acquiringFlow.AcquireSucceeded += AcquiringFlow_AcquireSucceeded;
            acquiringFlow.AcquireFailed += AcquiringFlow_AcquireFailed;
            acquiringFlow.StartAcquiring();
        }

        #endregion

        #region [Protected & private methods]

        /// <summary>
        /// Gets the acquiring flow of the shop product.
        /// </summary>
        protected abstract ShopProductAcquiringFlow CreateAcquiringFlow();

        protected virtual void OnAcquiringSucceeded()
        {
            AcquireSucceeded?.Invoke(this);
        }

        /// <summary>
        /// Acquire succeeded handler.
        /// </summary>
        private void AcquiringFlow_AcquireSucceeded()
        {
            OnAcquiringSucceeded();
        }

        private void AcquiringFlow_AcquireFailed()
        {
            AcquireFailed?.Invoke(this);
        }

        #endregion
    }
}
