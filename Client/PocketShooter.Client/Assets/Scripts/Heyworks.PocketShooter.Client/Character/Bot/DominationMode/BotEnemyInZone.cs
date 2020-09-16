using BehaviorDesigner.Runtime.Tasks;

namespace Heyworks.PocketShooter.Character.Bot.Actions
{
    [TaskCategory("Bot")]
    public class BotEnemyInZone : BotConditional
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
