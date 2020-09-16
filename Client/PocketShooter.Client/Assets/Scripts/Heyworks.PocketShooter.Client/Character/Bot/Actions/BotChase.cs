using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Heyworks.PocketShooter.Character.Bot.Actions
{
    [TaskCategory("Bot")]
    public class BotChase : BotNavMeshMovement
    {
        public override TaskStatus OnUpdate()
        {
            base.OnUpdate();

            if (Observer.CurrentTarget != null &&
                !Observer.AttackableCharacters.Contains(Observer.CurrentTarget.Player))
            {
                navMeshAgent.SetDestination(Observer.CurrentTarget.Player.Transform.Position);

                return TaskStatus.Running;
            }

            return TaskStatus.Success;
        }

        public override void OnEnd()
        {
            base.OnEnd();

            if (navMeshAgent.hasPath)
            {
                navMeshAgent.isStopped = true;
                navMeshAgent.ResetPath();
            }
        }
    }
}