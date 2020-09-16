using BehaviorDesigner.Runtime.Tasks;
using Heyworks.PocketShooter.Core;
using UnityEngine;

namespace Heyworks.PocketShooter.Character.Bot.Actions
{
    [TaskCategory("Bot")]
    public class BotMapLoading : Action
    {
        MapSceneManager mapSceneManager;

        public override void OnAwake()
        {
            base.OnAwake();

            mapSceneManager = Object.FindObjectOfType<MapSceneManager>();
        }

        public override TaskStatus OnUpdate()
        {
            if (mapSceneManager.IsMapLoaded)
            {
                return TaskStatus.Success;
            }

            return TaskStatus.Running;
        }
    }
}