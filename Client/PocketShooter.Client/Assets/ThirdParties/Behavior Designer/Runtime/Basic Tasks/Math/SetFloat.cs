﻿namespace BehaviorDesigner.Runtime.Tasks.Basic.Math
{
    [TaskCategory("Basic/Math")]
    [TaskDescription("Sets a float value")]
    public class SetFloat : Action
    {
        [Tooltip("The float value to set")]
        public SharedFloat floatValue;
        [Tooltip("The variable to store the result")]
        public SharedFloat storeResult;

        public override TaskStatus OnUpdate()
        {
            storeResult.Value = floatValue.Value;
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            floatValue.Value = 0;
            storeResult.Value = 0;
        }
    }
}