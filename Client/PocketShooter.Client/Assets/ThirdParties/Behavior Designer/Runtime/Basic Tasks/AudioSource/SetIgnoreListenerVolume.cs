﻿using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityAudioSource
{
    [TaskCategory("Basic/AudioSource")]
    [TaskDescription("Sets the ignore listener pause value of the AudioSource. Returns Success.")]
    public class SetIgnoreListenerPause : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The ignore listener pause value of the AudioSource")]
        public SharedBool ignoreListenerPause;

        private AudioSource audioSource;
        private GameObject prevGameObject;

        public override void OnStart()
        {
            var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
            if (currentGameObject != prevGameObject) {
                audioSource = currentGameObject.GetComponent<AudioSource>();
                prevGameObject = currentGameObject;
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (audioSource == null) {
                Debug.LogWarning("AudioSource is null");
                return TaskStatus.Failure;
            }

            audioSource.ignoreListenerPause = ignoreListenerPause.Value;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            ignoreListenerPause = false;
        }
    }
}
