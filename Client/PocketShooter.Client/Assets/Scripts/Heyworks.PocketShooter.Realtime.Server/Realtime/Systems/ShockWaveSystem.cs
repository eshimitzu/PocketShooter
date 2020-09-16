using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public class ShockWaveSystem : IServerInitiatorSystem
    {
        private readonly ITicker ticker;
        private readonly ApplyAoECommandData commandData;

        public ShockWaveSystem(ITicker ticker, ApplyAoECommandData commandData)
        {
            this.ticker = ticker;
            this.commandData = commandData;
        }

        public bool Execute(ServerPlayer initiator, IServerGame game)
        {
            var skillInfo = initiator.GetFirstSkillInfo<SkillInfo>(commandData.Skill);

            foreach (var victimId in commandData.Victims)
            {
                var victim = game.GetServerPlayer(victimId);
                if (victim != null)
                {
                    victim.Effects.Stun.IsStunned = true;
                    victim.StunExpire.ExpireAt = ticker.Current + skillInfo.ActiveTime;
                }
            }

            return true;
        }
    }
}
