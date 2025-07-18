﻿using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityRigidbody2D
{
    [TaskCategory("Basic/Rigidbody2D")]
    [TaskDescription("Forces the Rigidbody2D to wake up. Returns Success.")]
    public class WakeUp : Conditional
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;

        private Rigidbody2D rigidbody2D;
        private GameObject prevGameObject;

        public override void OnStart()
        {
            var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
            if (currentGameObject != prevGameObject) {
                rigidbody2D = currentGameObject.GetComponent<Rigidbody2D>();
                prevGameObject = currentGameObject;
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (rigidbody2D == null) {
                Debug.LogWarning("Rigidbody2D is null");
                return TaskStatus.Failure;
            }

            rigidbody2D.WakeUp();

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
        }
    }
}
