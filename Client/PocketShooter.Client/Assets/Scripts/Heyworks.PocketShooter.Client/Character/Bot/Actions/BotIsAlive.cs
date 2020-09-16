using BehaviorDesigner.Runtime.Tasks;

namespace Heyworks.PocketShooter.Character.Bot.Actions
{
    [TaskCategory("Bot")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}CanSeeObjectIcon.png")]
    public class BotIsAlive : BotConditional
    {
        public override TaskStatus OnUpdate()
        {
            if (Bot.Model.IsAlive)
            {
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }
    }
}