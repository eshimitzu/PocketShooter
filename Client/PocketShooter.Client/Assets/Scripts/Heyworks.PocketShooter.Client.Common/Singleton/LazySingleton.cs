using UnityEngine;

namespace Heyworks.PocketShooter.Singleton
{
    /// <summary>
    /// MonoBEhaviour singleton, whose instance is only created, when first requested.
    /// </summary>
    /// <typeparam name="T"> Type of the singleton MonoBehaviour. </typeparam>
    public class LazySingleton<T> : MonoBehaviour where T : LazySingleton<T>
    {
        private static T instance;

        /// <summary>
        /// Gets a static instance property that attempts to find the manager object in the scene and returns it to the caller.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    InitializeInstance();
                }

                return instance;
            }
        }

        /// <summary>
        /// Ensure that the instance is destroyed when the game is stopped in the editor.
        /// </summary>
        protected void OnApplicationQuit()
        {
            instance = null;
        }

        /// <summary>
        /// Handles a Unity OnDestroy event.
        /// </summary>
        protected virtual void OnDestroy()
        {
            instance = null;
        }

        private static void InitializeInstance()
        {
            instance = FindObjectOfType<T>();

            if (instance == null)
            {
                var obj = new GameObject(typeof(T).Name);
                instance = obj.AddComponent<T>();
            }
        }
    }
}
