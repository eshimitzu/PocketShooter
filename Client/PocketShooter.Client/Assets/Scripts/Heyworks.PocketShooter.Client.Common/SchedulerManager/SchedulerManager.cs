using System;
using System.Collections.Generic;
using Heyworks.PocketShooter.Singleton;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Heyworks.PocketShooter.Core.SchedulerManager
{
    /// <summary>
    /// Represents manager for scheduler tasks.
    /// </summary>
    public class SchedulerManager : LazyPersistentSingleton<SchedulerManager>
    {
        private List<SchedulerTask> schedulerTasks = new List<SchedulerTask>();
        private List<SchedulerTask> addingTasks = new List<SchedulerTask>();
        private List<SchedulerTask> removingTasks = new List<SchedulerTask>();

        #region Public methods

        /// <summary>
        /// Add new scheduler task.
        /// </summary>
        /// <param name="task">New task.</param>
        public void AddSchedulerTask(SchedulerTask task)
        {
            if (task != null)
            {
                addingTasks.Add(task);
            }
        }

        /// <summary>
        /// Remove scheduler task if exist.
        /// </summary>
        /// <param name="task">task for removing.</param>
        public void RemoveSchedulerTask(SchedulerTask task)
        {
            if (task != null)
            {
                removingTasks.Add(task);
            }
        }

        /// <summary>
        /// Execute action with delay.
        /// </summary>
        /// <param name="target">action target.</param>
        /// <param name="completingAction">action on completing scheduler.</param>
        /// <param name="delay">delay before executing action.</param>
        /// <param name="isReapiting">is need reapeting scheduler.</param>
        public SchedulerTask CallActionWithDelay(Object target, Action completingAction, float delay, bool isReapiting = false)
        {
            SchedulerTask task = new SchedulerTask(this, target, completingAction, delay, isReapiting);
            addingTasks.Add(task);

            return task;
        }

        #endregion

        #region Unity lifecycle

        private void Update()
        {
            if (addingTasks.Count > 0)
            {
                schedulerTasks.AddRange(addingTasks);
                addingTasks.Clear();
            }

            for (int i = 0; i < removingTasks.Count; i++)
            {
                schedulerTasks.Remove(removingTasks[i]);
            }

            removingTasks.Clear();

            SchedulerTask currentSchedulerTask;
            for (int i = 0; i < schedulerTasks.Count; i++)
            {
                currentSchedulerTask = schedulerTasks[i];

                if (currentSchedulerTask.IsNoTarget || currentSchedulerTask.Target != null)
                {
                    currentSchedulerTask.Update(Time.deltaTime);
                }
                else
                {
                    RemoveSchedulerTask(currentSchedulerTask);
                }
            }
        }

        #endregion
    }
}
