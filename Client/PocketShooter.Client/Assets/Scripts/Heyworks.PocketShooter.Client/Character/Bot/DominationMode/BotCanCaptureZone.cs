using BehaviorDesigner.Runtime.Tasks;

namespace Heyworks.PocketShooter.Character.Bot.Actions
{
    [TaskCategory("Bot")]
    public class BotCanCaptureZone : BotConditional
    {
        public override TaskStatus OnUpdate()
        {
            foreach (var zone in Bot.Simulation.Game.Zones.Values)
            {
                if (zone.State.OwnerTeam != Bot.Model.Team)
                {
                    return TaskStatus.Success;
                }
            }

            return TaskStatus.Failure;
        }
    }
}
