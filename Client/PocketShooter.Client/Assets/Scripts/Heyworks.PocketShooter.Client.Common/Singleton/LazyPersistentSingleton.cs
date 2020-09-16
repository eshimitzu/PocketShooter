using UnityEngine;

namespace Heyworks.PocketShooter.Singleton
{
    /// <summary>
    /// Singleton. To avoid having to manually link an instance to every class that needs it, it has a static property called Instance,
    /// so other objects that need to access it can just call: Manager.Instance.DoSomething();
    /// </summary>
    /// <typeparam name="T">Type of the singleton MonoBehaviour.</typeparam>
    public class LazyPersistentSingleton<T> : MonoBehaviour where T : LazyPersistentSingleton<T>
    {
        private static T instance;

        /// <summary>
        /// Gets a value indicating whether the instance of the singleton exists.
        /// </summary>
        public static bool HasInstance 
        {
            get
            {
                return instance != null;
            }
        }

        /// <summary>
        /// Gets a static instance property that attempts to find the manager object in the scene and
        /// returns it to the caller.
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
        protected virtual void OnApplicationQuit()
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

            if (instance)
            {
                DontDestroyOnLoad(instance.gameObject);
            }
        }
    }
}