using BehaviorDesigner.Runtime.Tasks;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;

namespace Heyworks.PocketShooter.Character.Bot.Actions
{
    [TaskCategory("Bot")]
    public class BotHeal : BotAction
    {
        public override TaskStatus OnUpdate()
        {
            var commandData = new UseSkillCommandData(Bot.Model.Id, SkillName.MedKit);
            Bot.AddCommand(commandData);

            return TaskStatus.Success;
        }
    }
}