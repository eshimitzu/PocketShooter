using System.Collections.Generic;
using System.Linq;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;
using UnityEngine;

namespace Heyworks.PocketShooter.Character.Bot.Actions
{
    [TaskCategory("Bot")]
    public class BotWander : Wander
    {
        [SerializeField]
        private CharacterControllerSettings settings = null;

        public override void OnAwake()
        {
            base.OnAwake();

            navMeshAgent.speed = settings.MaxStableMoveSpeed;
        }
    }
}
