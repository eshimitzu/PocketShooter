﻿using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityLight
{
    [TaskCategory("Basic/Light")]
    [TaskDescription("Stores the intensity of the light.")]
    public class GetIntensity : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [RequiredField]
        [Tooltip("The intensity to store")]
        public SharedFloat storeValue;

        // cache the light component
        private Light light;
        private GameObject prevGameObject;

        public override void OnStart()
        {
            var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
            if (currentGameObject != prevGameObject) {
                light = currentGameObject.GetComponent<Light>();
                prevGameObject = currentGameObject;
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (light == null) {
                Debug.LogWarning("Light is null");
                return TaskStatus.Failure;
            }

            storeValue = light.intensity;
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            storeValue = 0;
        }
    }
}