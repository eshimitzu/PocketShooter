using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace Heyworks.PocketShooter.Character.Bot.Actions
{
    [TaskDescription("The selector task is similar if. Execute A if true, B if false")]
    [TaskIcon("{SkinColor}SelectorIcon.png")]
    public class Switch : Composite
    {
        [Tooltip("Should the conditional task be reevaluated every tick?")]
        public SharedBool reevaluate;

        [InspectTask]
        [Tooltip("The conditional task to evaluate")]
        public Conditional conditionalTask;

        private bool checkConditionalTask = true;

        private bool conditionalTaskFailed = false;

        // The index of the child that is currently running or is about to run.
        private int currentChildIndex = 0;

        // The task status of the last child ran.
        private TaskStatus executionStatus = TaskStatus.Inactive;

        public override void OnAwake()
        {
            if (conditionalTask != null)
            {
                conditionalTask.Owner = Owner;
                conditionalTask.GameObject = gameObject;
                conditionalTask.Transform = transform;
                conditionalTask.OnAwake();
            }
        }

        public override void OnReset()
        {
            conditionalTask = null;
        }

        public override void OnStart()
        {
            if (conditionalTask != null)
            {
                conditionalTask.OnStart();
            }
        }

        public override int CurrentChildIndex()
        {
            return currentChildIndex;
        }

        public override int MaxChildren()
        {
            return 2;
        }

        public override void OnConditionalAbort(int childIndex)
        {
            // Set the current child index to the index that caused the abort
            currentChildIndex = childIndex;
            executionStatus = TaskStatus.Inactive;
        }

        public override bool CanReevaluate()
        {
            return reevaluate.Value;
        }

        public override bool CanExecute()
        {
            if (checkConditionalTask)
            {
                checkConditionalTask = false;
                OnSwitch();
            }

            // We can continue to execuate as long as we have children that haven't been executed and no child has returned success.
            return currentChildIndex < children.Count && executionStatus != TaskStatus.Success;
        }

        public override void OnChildExecuted(TaskStatus childStatus)
        {
            executionStatus = childStatus;
        }

        public override void OnEnd()
        {
            executionStatus = TaskStatus.Inactive;
            checkConditionalTask = true;
            conditionalTaskFailed = false;
//            currentChildIndex = 0;
            if (conditionalTask != null)
            {
                conditionalTask.OnEnd();
            }
        }

        public override TaskStatus OverrideStatus()
        {
            return TaskStatus.Failure;
        }

        public override TaskStatus OverrideStatus(TaskStatus status)
        {
            if (conditionalTaskFailed)
                return TaskStatus.Failure;
            return status;
        }

        protected virtual void OnSwitch()
        {
            UnityEngine.Debug.Log($"OnSwitch");
            TaskStatus childStatus = conditionalTask.OnUpdate();
            currentChildIndex = childStatus == TaskStatus.Success ? 0 : 1;
        }
    }
}