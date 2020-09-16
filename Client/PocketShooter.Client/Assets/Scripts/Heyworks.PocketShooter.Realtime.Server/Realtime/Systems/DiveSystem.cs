using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public class DiveSystem : IServerInitiatorSystem
    {
        private readonly ApplyAoECommandData commandData;

        public DiveSystem(ITicker ticker, ApplyAoECommandData commandData)
        {
            this.commandData = commandData;
        }

        public bool Execute(ServerPlayer initiator, IServerGame game)
        {
            var skill = initiator.GetFirstOwnedSkill(commandData.Skill);
            var skillInfo = (DiveSkillInfo)skill.Info;

            foreach (var victimId in commandData.Victims)
            {
                var victim = game.GetServerPlayer(victimId);
                victim?.Damages.Add(
                    new DamageInfo(
                        initiator.Id,
                        new EntityRef(EntityType.Skill, skill.Skill.Id.Item1),
                        DamageType.Splash,
                        skillInfo.Damage));
            }

            return true;
        }
    }
}
