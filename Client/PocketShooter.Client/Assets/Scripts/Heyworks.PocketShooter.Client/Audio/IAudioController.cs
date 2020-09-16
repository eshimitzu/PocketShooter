using UnityEngine;

namespace Heyworks.PocketShooter.Audio
{
    /// <summary>
    /// Represents interface for working with audio.
    /// </summary>
    public interface IAudioController
    {
        /// <summary>
        /// Posts the event.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="gameObject">The game object.</param>
        void PostEvent(string eventName, GameObject gameObject);

        /// <summary>
        /// Posts the event.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        void PostUIEvent(string eventName);

        /// <summary>
        /// Sets the sate.
        /// </summary>
        /// <param name="stateGroup">The state group.</param>
        /// <param name="state">The state.</param>
        void SetState(string stateGroup, string state);

        /// <summary>
        /// Set rtpc value.
        /// </summary>
        /// <param name="rtpcName">The rtpc name.</param>
        /// <param name="value">The rtpc value.</param>
        /// <param name="scope">The scope.</param>
        void SetRTPC(string rtpcName, float value, GameObject scope);
    }
}
