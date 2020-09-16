using System;
using Object = UnityEngine.Object;

namespace Heyworks.PocketShooter.Core.SchedulerManager
{
    /// <summary>
    /// Represents scheduler task for SchedulerManager.
    /// </summary>
    public class SchedulerTask
    {
        /// <summary>
        /// Completing action.
        /// </summary>
        private Action action;

        /// <summary>
        /// Gets object, where executing action.
        /// </summary>
        public Object Target { get; private set; }

        /// <summary>
        /// Gets a value indicating whether is exist target.
        /// </summary>
        public bool IsNoTarget { get; private set; }

        /// <summary>
        /// Gets or sets time before execute action.
        /// </summary>
        public float Delay { get; set; }

        /// <summary>
        /// Gets or sets elapsed time before execute action.
        /// </summary>
        public float ElapsedTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is need reapeting task.
        /// </summary>
        private bool IsReapeting { get; set; }

        private SchedulerManager schedulerManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulerTask"/> class.
        /// </summary>
        /// <param name="schedulerManager">The scheduler manager.</param>
        /// <param name="target">The target.</param>
        /// <param name="action">The action.</param>
        /// <param name="delay">The delay.</param>
        /// <param name="isReapeting">if set to <c>true</c> [is reapeting].</param>
        public SchedulerTask(SchedulerManager schedulerManager, Object target, Action action, float delay, bool isReapeting = false)
        {
            this.IsNoTarget = target == null;
            this.Target = target;
            this.action = action;
            this.Delay = delay;
            this.IsReapeting = isReapeting;
            this.schedulerManager = schedulerManager;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulerTask"/> class.
        /// </summary>
        /// <param name="schedulerManager">The scheduler manager.</param>
        /// <param name="action">The action.</param>
        /// <param name="delay">The delay.</param>
        /// <param name="isReapeting">if set to <c>true</c> [is reapeting].</param>
        public SchedulerTask(SchedulerManager schedulerManager, Action action, float delay, bool isReapeting = false)
        {
            this.IsNoTarget = true;
            this.action = action;
            this.Delay = delay;
            this.IsReapeting = isReapeting;
            this.schedulerManager = schedulerManager;
        }

        /// <summary>
        /// Updates with the specified delta time.
        /// </summary>
        /// <param name="deltaTime">The delta time.</param>
        public void Update(float deltaTime)
        {
            ElapsedTime += deltaTime;

            if (ElapsedTime >= Delay)
            {
                action();

                if (IsReapeting)
                {
                    ElapsedTime = 0f;
                }
                else
                {
                    schedulerManager.RemoveSchedulerTask(this);
                }
            }
        }
    }
}