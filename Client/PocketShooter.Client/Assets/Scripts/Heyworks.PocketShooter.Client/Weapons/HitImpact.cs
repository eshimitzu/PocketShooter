using Heyworks.PocketShooter.Audio;
using Lean.Pool;
using UnityEngine;

namespace Heyworks.PocketShooter.Weapons
{
    public class HitImpact : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem particles = null;
        [SerializeField]
        private string sfxEventName = null;

        public void Play()
        {
            particles.Play();
            AudioController.Instance.PostEvent(sfxEventName, gameObject);

            Invoke("Stop", 1f);
        }

        public void Stop()
        {
            LeanPool.Despawn(gameObject);
        }
    }
}
