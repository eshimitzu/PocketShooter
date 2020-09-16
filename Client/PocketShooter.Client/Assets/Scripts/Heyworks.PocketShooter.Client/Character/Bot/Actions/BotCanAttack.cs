using BehaviorDesigner.Runtime.Tasks;

namespace Heyworks.PocketShooter.Character.Bot.Actions
{
    [TaskCategory("Bot")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}CanSeeObjectIcon.png")]
    public class BotCanAttack : BotConditional
    {
        public override TaskStatus OnUpdate()
        {
            if (Observer.CurrentTarget != null)
            {
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }
    }
}