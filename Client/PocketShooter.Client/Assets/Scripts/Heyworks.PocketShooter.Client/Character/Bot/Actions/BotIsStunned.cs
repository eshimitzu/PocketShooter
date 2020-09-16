using BehaviorDesigner.Runtime.Tasks;
using Heyworks.PocketShooter.Realtime.Entities;

namespace Heyworks.PocketShooter.Character.Bot.Actions
{
    [TaskCategory("Bot")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}CanSeeObjectIcon.png")]
    public class BotIsStunned : BotConditional
    {
        public override TaskStatus OnUpdate()
        {
            if (Bot.Model is ClientPlayer clientPlayer && clientPlayer.IsStunned)
            {
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }
    }
}