using UnityEngine;

namespace Heyworks.PocketShooter.Audio
{
    /// <summary>
    /// Represents class for handling animation events and playing sfx.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class AudioEventHandler : MonoBehaviour
    {
        /// <summary>
        /// Posts the event.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        public void Fire(string eventName)
        {
            AudioController.Instance.PostEvent(eventName, gameObject);
        }
    }
}
