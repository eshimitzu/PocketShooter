using System.Collections.Generic;
using System.Linq;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;
using Heyworks.PocketShooter.Realtime.Entities;
using UnityEngine;

namespace Heyworks.PocketShooter.Character.Bot.Actions
{
    [TaskCategory("Bot")]
    public class BotMansing : BotNavMeshMovement
    {
        [SerializeField]
        private float turnSpeed = 10;

        public SharedFloat MinWanderDistance = 1;
        public SharedFloat MaxWanderDistance = 2;
        public SharedFloat WanderRate = 2;
        public SharedFloat MinPauseDuration = 0;
        public SharedFloat MaxPauseDuration = 0;
        public SharedInt TargetRetries = 1;

        public override TaskStatus OnUpdate()
        {
            if (HasArrived())
            {
                TrySetTarget();
            }

            IRemotePlayer target = Observer.CurrentTarget?.Player;
            if (target != null)
            {
                Vector3 forward = target.Transform.Position - Bot.transform.position;
                Vector3 tr = Quaternion.LookRotation(forward, Vector3.up).eulerAngles;

                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, tr.y, tr.z), Time.deltaTime * turnSpeed);
            }

            return TaskStatus.Running;
        }

        private bool TrySetTarget()
        {
            var validDestination = false;
            var attempts = TargetRetries.Value;
            var destination = transform.position;
            while (!validDestination && attempts > 0)
            {
                var direction = Random.Range(0, 100) < 50 ? -transform.right : transform.right;

                // direction = direction + Random.insideUnitSphere * wanderRate.Value;
                destination = transform.position + direction.normalized * Random.Range(MinWanderDistance.Value, MaxWanderDistance.Value);
                validDestination = SamplePosition(destination);
                attempts--;
            }

            if (validDestination)
            {
                SetDestination(destination);
            }

            return validDestination;
        }
    }
}
