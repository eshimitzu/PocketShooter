﻿using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityRigidbody2D
{
    [TaskCategory("Basic/Rigidbody2D")]
    [TaskDescription("Sets the drag of the Rigidbody2D. Returns Success.")]
    public class SetDrag : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The drag of the Rigidbody2D")]
        public SharedFloat drag;

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

            rigidbody2D.drag = drag.Value;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            drag = 0;
        }
    }
}
