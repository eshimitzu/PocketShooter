using UnityEngine;

namespace Heyworks.PocketShooter.Audio
{
    public abstract class CharacterAudioController : MonoBehaviour
    {
        public abstract void HandleStun(bool isStunned);

        public abstract void HandleLifestealPlus();

        public abstract void HandleLifestealMinus();

        public abstract void HandleHeal();

        public abstract void HandleRegeneration();

        public abstract void HandleInstantReload();

        public abstract void HandleRunSprint();

        public abstract void HandleStopSprint();

        public void PostEvent(string eventName)
        {
            AudioController.Instance.PostEvent(eventName, gameObject);
        }
    }
}
