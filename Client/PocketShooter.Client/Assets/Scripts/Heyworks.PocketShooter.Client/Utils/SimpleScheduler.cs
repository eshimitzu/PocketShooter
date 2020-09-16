using System;
using UnityEngine;

namespace Heyworks.PocketShooter.Utils
{
    public class SimpleScheduler
    {
        private Action actionToExecute;
        private float period;

        private float lastUpdateTime;

        public SimpleScheduler(Action action, float period)
        {
            AssertUtils.NotNull(action, nameof(action));

            actionToExecute = action;
            this.period = period;
        }

        public void TryExecute()
        {
            if (lastUpdateTime + period < Time.time)
            {
                lastUpdateTime = Time.time;

                actionToExecute();
            }
        }
    }
}
