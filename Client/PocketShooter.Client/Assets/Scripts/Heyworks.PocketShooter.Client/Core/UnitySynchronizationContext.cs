using System;
using System.Threading;
using UniRx;

namespace Heyworks.PocketShooter.Core
{
    /// <summary>
    /// Synchronization context for Unity. Use <see cref="MainThreadDispatcher"/> internally. Singleton class.
    /// </summary>
    public sealed class UnitySynchronizationContext : SynchronizationContext
    {
        private static readonly UnitySynchronizationContext instance;

        static UnitySynchronizationContext()
        {
            instance = new UnitySynchronizationContext();
        }

        private UnitySynchronizationContext()
        {
        }

        /// <summary>
        /// Gets the instance of this class.
        /// </summary>
        public static new SynchronizationContext Current => instance;

        /// <summary>
        /// Operation is not implemented.
        /// </summary>
        /// <param name="syncContext">The <see cref="SynchronizationContext"/> object to be set.</param>
        public static new void SetSynchronizationContext(SynchronizationContext syncContext)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Dispatches an asynchronous message to a synchronization context.
        /// </summary>
        /// <param name="d">The <see cref="SendOrPostCallback "/> delegate to call.</param>
        /// <param name="state">The object passed to the delegate.</param>
        public override void Post(SendOrPostCallback d, object state)
        {
            MainThreadDispatcher.Post(new Action<object>(d), state);
        }

        /// <summary>
        /// Dispatches a synchronous message to a synchronization context.
        /// </summary>
        /// <param name="d">The <see cref="SendOrPostCallback "/> delegate to call.</param>
        /// <param name="state">The object passed to the delegate.</param>
        public override void Send(SendOrPostCallback d, object state)
        {
            MainThreadDispatcher.Send(new Action<object>(d), state);
        }

        /// <summary>
        /// Operation is not implemented.
        /// </summary>
        public override SynchronizationContext CreateCopy()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Operation is not implemented.
        /// </summary>
        public override void OperationStarted()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Operation is not implemented.
        /// </summary>
        public override void OperationCompleted()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Operation is not implemented.
        /// </summary>
        /// <param name="waitHandles">An array of type <see cref="IntPtr"/> that contains the native operating system handles.</param>
        /// <param name="waitAll">true to wait for all handles; false to wait for any handle.</param>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or <see cref="Timeout.Infinite"/> (-1) to wait indefinitely.</param>
        public override int Wait(IntPtr[] waitHandles, bool waitAll, int millisecondsTimeout)
        {
            throw new NotImplementedException();
        }
    }
}
