﻿using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityRigidbody
{
    [TaskCategory("Basic/Rigidbody")]
    [TaskDescription("Applies a force to the rigidbody relative to its coordinate system. Returns Success.")]
    public class AddRelativeForce : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The amount of force to apply")]
        public SharedVector3 force;
        [Tooltip("The type of force")]
        public ForceMode forceMode = ForceMode.Force;

        // cache the rigidbody component
        private Rigidbody rigidbody;
        private GameObject prevGameObject;

        public override void OnStart()
        {
            var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
            if (currentGameObject != prevGameObject) {
                rigidbody = currentGameObject.GetComponent<Rigidbody>();
                prevGameObject = currentGameObject;
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (rigidbody == null) {
                Debug.LogWarning("Rigidbody is null");
                return TaskStatus.Failure;
            }

            rigidbody.AddRelativeForce(force.Value, forceMode);

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            force = Vector3.zero;
            forceMode = ForceMode.Force;
        }
    }
}