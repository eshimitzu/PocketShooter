using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading;

namespace Heyworks.PocketShooter.Utils
{
    /// <summary>
    /// Extension methods for <see cref="HubConnection"/>.
    /// </summary>
    public static class HubConnectionExtensions
    {
        /// <summary>
        /// Registers a handler that will be invoked when the hub method with the specified method name is invoked.
        /// Handler will be dispatched to the provided synchronization context.
        /// </summary>
        /// <typeparam name="T1">The first argument type.</typeparam>
        /// <param name="hubConnection">The hub connection.</param>
        /// <param name="methodName">The name of the hub method to define.</param>
        /// <param name="handler">The handler that will be raised when the hub method is invoked.</param>
        /// <param name="synchronizationContext">The synchronization context.</param>
        /// <returns>A subscription that can be disposed to unsubscribe from the hub method.</returns>
        public static IDisposable OnWithContext<T1>(
            this HubConnection hubConnection,
            string methodName,
            Action<T1> handler,
            SynchronizationContext synchronizationContext)
        {
            return hubConnection.On<T1>(
                methodName,
                (obj1) => synchronizationContext.Post(_ => handler(obj1), null));
        }

        /// <summary>
        /// Registers a handler that will be invoked when the hub method with the specified method name is invoked.
        /// Handler will be dispatched to the provided synchronization context.
        /// </summary>
        /// <typeparam name="T1">The first argument type.</typeparam>
        /// <typeparam name="T2">The second argument type.</typeparam>
        /// <param name="hubConnection">The hub connection.</param>
        /// <param name="methodName">The name of the hub method to define.</param>
        /// <param name="handler">The handler that will be raised when the hub method is invoked.</param>
        /// <param name="synchronizationContext">The synchronization context.</param>
        /// <returns>A subscription that can be disposed to unsubscribe from the hub method.</returns>
        public static IDisposable OnWithContext<T1, T2>(
            this HubConnection hubConnection,
            string methodName,
            Action<T1, T2> handler,
            SynchronizationContext synchronizationContext)
        {
            return hubConnection.On<T1, T2>(
                methodName,
                (obj1, obj2) => synchronizationContext.Post(_ => handler(obj1, obj2), null));
        }

        /// <summary>
        /// Registers a handler that will be invoked when the hub method with the specified method name is invoked.
        /// Handler will be dispatched to the provided synchronization context.
        /// </summary>
        /// <typeparam name="T1">The first argument type.</typeparam>
        /// <typeparam name="T2">The second argument type.</typeparam>
        /// <typeparam name="T3">The third argument type.</typeparam>
        /// <param name="hubConnection">The hub connection.</param>
        /// <param name="methodName">The name of the hub method to define.</param>
        /// <param name="handler">The handler that will be raised when the hub method is invoked.</param>
        /// <param name="synchronizationContext">The synchronization context.</param>
        /// <returns>A subscription that can be disposed to unsubscribe from the hub method.</returns>
        public static IDisposable OnWithContext<T1, T2, T3>(
            this HubConnection hubConnection,
            string methodName,
            Action<T1, T2, T3> handler,
            SynchronizationContext synchronizationContext)
        {
            return hubConnection.On<T1, T2, T3>(
                methodName,
                (obj1, obj2, obj3) => synchronizationContext.Post(_ => handler(obj1, obj2, obj3), null));
        }
    }
}
