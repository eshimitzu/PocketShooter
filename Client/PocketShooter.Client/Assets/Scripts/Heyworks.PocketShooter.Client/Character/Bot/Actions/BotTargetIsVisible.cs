using BehaviorDesigner.Runtime.Tasks;

namespace Heyworks.PocketShooter.Character.Bot.Actions
{
    [TaskCategory("Bot")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}CanSeeObjectIcon.png")]
    public class BotTargetIsVisible : BotConditional
    {
        public override TaskStatus OnUpdate()
        {
            if (Observer.CurrentTarget.IsVisible)
            {
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }
    }
}