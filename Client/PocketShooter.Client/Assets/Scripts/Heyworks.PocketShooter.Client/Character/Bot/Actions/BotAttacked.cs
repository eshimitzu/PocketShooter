using BehaviorDesigner.Runtime.Tasks;

namespace Heyworks.PocketShooter.Character.Bot.Actions
{
    [TaskCategory("Bot")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}CanSeeObjectIcon.png")]
    public class BotAttacked : BotConditional
    {
        public override TaskStatus OnUpdate()
        {
            if (Bot.Model.Damages.Count > 0)
            {
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }
    }
}