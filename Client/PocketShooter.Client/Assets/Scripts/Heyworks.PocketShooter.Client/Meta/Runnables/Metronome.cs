using System;
using Heyworks.PocketShooter.Singleton;
using UnityEngine;

namespace Heyworks.PocketShooter.Meta.Runnables
{
    /// <summary>
    /// Represents an object which produces regular, metrical ticks.
    /// </summary>
    public sealed class Metronome : LazyPersistentSingleton<Metronome>, IMetronome
    {
        private const float TICK_TIME = 1;
        private float lastTimeStamp;

        /// <summary>
        /// Gets a tick interval in seconds.
        /// </summary>
        public float TickIntervalSeconds
        {
            get
            {
                return TICK_TIME;
            }
        }

        /// <summary>
        /// Fires every n seconds.
        /// </summary>
        public event Action Tick;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            lastTimeStamp = Time.time;

            //gameObject.hideFlags = HideFlags.HideInHierarchy;
        }

        // Update is called once per frame
        private void Update()
        {
            if ((Time.time - lastTimeStamp) > TickIntervalSeconds)
            {
                lastTimeStamp = Time.time;
                OnTick();
            }
        }

        private void OnTick()
        {
            var handler = Tick;
            if (handler != null)
            {
                handler();
            }
        }
    }
}