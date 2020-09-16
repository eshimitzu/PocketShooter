using Heyworks.PocketShooter.Singleton;
using Microsoft.Extensions.Logging;
using UnityEngine;

namespace Heyworks.PocketShooter.Audio
{
    /// <summary>
    /// Represents audio controller for working with Wwise.
    /// </summary>
    public class AudioController : Singleton<AudioController>, IAudioController
    {
        [SerializeField]
        private GameObject uiAudioSource;

        /// <summary>
        /// Posts the event.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="sourceGameObject">The source game object.</param>
        public void PostEvent(string eventName, GameObject sourceGameObject)
        {
            AkSoundEngine.PostEvent(eventName, sourceGameObject);

            AudioLog.Log.LogTrace("Post Wwise event {eventName}.", eventName);
        }

        /// <summary>
        /// Posts the event.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        public void PostEvent(string eventName)
        {
            PostEvent(eventName, gameObject);
        }

        /// <summary>
        /// Posts the event.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        public void PostUIEvent(string eventName)
        {
            PostEvent(eventName, uiAudioSource);
        }

        /// <summary>
        /// Set rtpc value.
        /// </summary>
        /// <param name="rtpcName">The rtpc name.</param>
        /// <param name="value">The rtpc value.</param>
        /// <param name="scope">The scope.</param>
        public void SetRTPC(string rtpcName, float value, GameObject scope)
        {
            AkSoundEngine.SetRTPCValue(rtpcName, value, scope);
        }

        /// <summary>
        /// Sets the sate.
        /// </summary>
        /// <param name="stateGroup">The state group.</param>
        /// <param name="state">The state.</param>
        public void SetState(string stateGroup, string state)
        {
            AkSoundEngine.SetState(stateGroup, state);

            AudioLog.Log.LogTrace("Set Wwise state. State group: {stateGroup}, state: {state}.", stateGroup, state);
        }
    }
}
