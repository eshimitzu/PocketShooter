using BehaviorDesigner.Runtime.Tasks;

namespace Heyworks.PocketShooter.Character.Bot.Actions
{
    [TaskCategory("Bot")]
    public class BotCapturingZone : BotConditional
    {
        public override TaskStatus OnUpdate()
        {
            foreach (var zone in Bot.Simulation.Game.Zones.Values)
            {
                if (zone.IsInside(Bot.Model) && zone.State.OwnerTeam != Bot.Model.Team)
                {
                    return TaskStatus.Success;
                }
            }

            return TaskStatus.Failure;
        }
    }
}
