using BehaviorDesigner.Runtime.Tasks;
using Heyworks.PocketShooter.Realtime.Entities;

namespace Heyworks.PocketShooter.Character.Bot.Actions
{
    [TaskCategory("Bot")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}CanSeeObjectIcon.png")]
    public class BotSeeEnemy : BotConditional
    {
        public override TaskStatus OnUpdate()
        {
            if (Bot.Observer.VisibleCharacters.Count > 0)
            {
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }
    }
}