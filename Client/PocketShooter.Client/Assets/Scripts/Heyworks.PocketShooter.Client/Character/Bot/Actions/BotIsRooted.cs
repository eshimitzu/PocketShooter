using BehaviorDesigner.Runtime.Tasks;
using Heyworks.PocketShooter.Realtime.Entities;
using UnityEngine;

namespace Heyworks.PocketShooter.Character.Bot.Actions
{
    [TaskCategory("Bot")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}CanSeeObjectIcon.png")]
    public class BotIsRooted : BotConditional
    {
        public override TaskStatus OnUpdate()
        {
            if (Bot.Model is ClientPlayer clientPlayer && clientPlayer.IsRooted)
            {
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }
    }
}
