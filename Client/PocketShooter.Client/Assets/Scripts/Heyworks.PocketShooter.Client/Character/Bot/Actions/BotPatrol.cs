using System.Collections.Generic;
using System.Linq;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;
using UnityEngine;

namespace Heyworks.PocketShooter.Character.Bot.Actions
{
    [TaskCategory("Bot")]
    public class BotPatrol : Patrol
    {
        [SerializeField]
        private CharacterControllerSettings settings = null;

        public override void OnAwake()
        {
            GameObject[] scenePoints = GameObject.FindGameObjectsWithTag("Waypoint");
            waypoints.Value = new List<GameObject>(scenePoints.Select(g => g.gameObject));

            base.OnAwake();

            navMeshAgent.speed = settings.MaxStableMoveSpeed;
        }
    }
}
