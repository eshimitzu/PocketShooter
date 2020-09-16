using System.Linq;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Entities.Skills;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public class ActivateImmortalitySkillSystem : SkillSystem<ICooldownSkillForSystem, SkillInfo>
    {
        protected override SkillName SkillName => SkillName.Immortality;

        public ActivateImmortalitySkillSystem(ITicker ticker)
            : base(ticker)
        {
        }

        protected override void Execute(ICooldownSkillForSystem skill, SkillInfo skillInfo, ServerPlayer initiator, IServerGame game)
        {
            if (skill.State == SkillState.Default &&
                initiator.Damages.Any())
            {
                initiator.Effects.Immortal.IsImmortal = true;
                initiator.ImmortalExpire.ExpireAt = Ticker.Current + skillInfo.ActiveTime;

                skill.State = SkillState.Active;
                skill.StateExpireAt = Ticker.Current + skillInfo.ActiveTime;
            }
        }
    }
}