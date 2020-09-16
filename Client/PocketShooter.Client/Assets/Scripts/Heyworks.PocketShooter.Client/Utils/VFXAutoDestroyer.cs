using Lean.Pool;
using UnityEngine;

namespace Heyworks.PocketShooter.Utils
{
    public class VFXAutoDestroyer : MonoBehaviour
    {
        public event System.Action OnStopAlive;

        private ParticleSystem mainParticleSystem;

        private void Start()
        {
            mainParticleSystem = GetComponentInChildren<ParticleSystem>();
        }

        private void Update()
        {
            if (!mainParticleSystem.IsAlive(true))
            {
                if (gameObject.activeSelf)
                {
                    OnStopAlive?.Invoke();

                    LeanPool.Despawn(gameObject);

                    Destroy(this);
                }
            }
        }
    }
}
