using BehaviorDesigner.Runtime.Tasks;
using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Character.Bot.Actions
{
    [TaskCategory("Bot")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}CanSeeObjectIcon.png")]
    public class BotCanHeal : BotConditional
    {
        public override TaskStatus OnUpdate()
        {
            if (Bot.Model.CanUseFirstAidKit())
            {
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }
    }
}