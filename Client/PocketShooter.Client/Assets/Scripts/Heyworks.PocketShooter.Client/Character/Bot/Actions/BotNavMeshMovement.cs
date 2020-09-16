using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks.Movement;
using Heyworks.PocketShooter.Networking.Actors;
using UnityEngine;
using UnityEngine.AI;

namespace Heyworks.PocketShooter.Character.Bot.Actions
{
    /// <summary>
    /// Fork of NavMeshMovement.
    /// </summary>
    public abstract class BotNavMeshMovement : Movement
    {
        [Tooltip("The angular speed of the agent")]
        public SharedFloat angularSpeed = 120;

        [Tooltip("The agent has arrived when the destination is less than the specified amount. This distance should be greater than or equal to the NavMeshAgent StoppingDistance.")]
        public SharedFloat arriveDistance = 0.2f;

        [Tooltip("Should the NavMeshAgent be stopped when the task ends?")]
        public SharedBool stopOnTaskEnd = true;

        [Tooltip("Should the NavMeshAgent rotation be updated when the task ends?")]
        public SharedBool updateRotation = true;

        // Component references
        protected NavMeshAgent navMeshAgent;
        private bool startUpdateRotation;

        protected EnemyObserver Observer { get; private set; }

        protected BotCharacter Bot { get; private set; }

        public override void OnAwake()
        {
            base.OnAwake();

            navMeshAgent = GetComponent<NavMeshAgent>();
            Observer = gameObject.GetComponent<EnemyObserver>();
            Bot = gameObject.GetComponent<BotCharacter>();
        }

        public override void OnStart()
        {
            navMeshAgent.angularSpeed = angularSpeed.Value;
            navMeshAgent.isStopped = false;
            startUpdateRotation = navMeshAgent.updateRotation;
            UpdateRotation(updateRotation.Value);
        }

        protected override bool SetDestination(Vector3 destination)
        {
            navMeshAgent.isStopped = false;
            return navMeshAgent.SetDestination(destination);
        }

        protected override void UpdateRotation(bool update)
        {
            navMeshAgent.updateRotation = update;
            navMeshAgent.updateUpAxis = update;
        }

        protected override bool HasPath()
        {
            return navMeshAgent.hasPath && navMeshAgent.remainingDistance > arriveDistance.Value;
        }

        protected override Vector3 Velocity()
        {
            return navMeshAgent.velocity;
        }

        protected bool SamplePosition(Vector3 position)
        {
            return NavMesh.SamplePosition(position, out NavMeshHit hit, navMeshAgent.height * 2, NavMesh.AllAreas);
        }

        protected override bool HasArrived()
        {
            // The path hasn't been computed yet if the path is pending.
            float remainingDistance;
            if (navMeshAgent.pathPending)
            {
                remainingDistance = float.PositiveInfinity;
            }
            else
            {
                remainingDistance = navMeshAgent.remainingDistance;
            }

            return remainingDistance <= arriveDistance.Value;
        }

        protected override void Stop()
        {
            UpdateRotation(startUpdateRotation);
            if (navMeshAgent.hasPath)
            {
                navMeshAgent.isStopped = true;
            }
        }

        public override void OnEnd()
        {
            if (stopOnTaskEnd.Value)
            {
                Stop();
            }
            else
            {
                UpdateRotation(startUpdateRotation);
            }
        }

        public override void OnBehaviorComplete()
        {
            Stop();
        }

        public override void OnReset()
        {
            angularSpeed = 120;
            arriveDistance = 1;
            stopOnTaskEnd = true;
        }
    }
}