using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Entities.Skills;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public abstract class SkillSystem<TSkill, TSkillInfo> : ServerInitiatorSystem
        where TSkill : class, ISkillForSystem
        where TSkillInfo : SkillInfo
    {
        protected ITicker Ticker { get; }

        protected abstract SkillName SkillName { get; }

        protected SkillSystem(ITicker ticker)
        {
            this.Ticker = ticker;
        }

        protected abstract void Execute(TSkill skill, TSkillInfo skillInfo, ServerPlayer initiator, IServerGame game);

        protected override void Execute(ServerPlayer initiator, IServerGame game)
        {
            var (skill, info) = initiator.GetFirstSkillWithInfo<TSkill, TSkillInfo>(SkillName);

            if (skill != null && info != null)
            {
                Execute(skill, info, initiator, game);
            }
        }
    }
}