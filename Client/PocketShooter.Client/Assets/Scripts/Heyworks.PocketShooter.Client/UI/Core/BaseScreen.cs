using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Heyworks.PocketShooter.UI.Core
{
    /// <summary>
    /// Represents base screen for all ui screens.
    /// </summary>
    public abstract class BaseScreen : MonoBehaviour
    {
        private readonly IList<IDisposablePresenter> disposables = new List<IDisposablePresenter>();

        /// <summary>
        /// Called when screen is destroyed.
        /// </summary>
        protected virtual void OnDestroy()
        {
            DisposePresenters();
        }

        public void Hide()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// Adds the disposable presenter to the disposable collection. Will be disposed OnDestroy.
        /// </summary>
        /// <param name="disposable">The disposable.</param>
        protected void AddDisposablePresenter(IDisposablePresenter disposable)
        {
            disposables.Add(disposable);
        }

        /// <summary>
        /// Dispose all added presenters and clear the disposable collection.
        /// </summary>
        protected void DisposePresenters()
        {
            foreach (var disposable in disposables)
            {
                disposable.Dispose();
            }

            disposables.Clear();
        }

        /// <summary>
        /// Represents factory for all UI screens.
        /// </summary>
        public class Factory : PlaceholderFactory<Object, BaseScreen>
        {
        }
    }
}
