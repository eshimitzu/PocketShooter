using System;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Entities.Skills;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public class LifestealSkillSystem : SkillSystem<ILifestealSkillForSystem, LifestealSkillInfo>
    {
        protected override SkillName SkillName => SkillName.Lifesteal;

        public LifestealSkillSystem(ITicker ticker)
            : base(ticker)
        {
        }

        protected override void Execute(ILifestealSkillForSystem skill, LifestealSkillInfo skillInfo, ServerPlayer initiator, IServerGame game)
        {
            if (initiator.IsAlive && skill.State == SkillState.Active && skill.NextStealTime <= Ticker.Current)
            {
                skill.NextStealTime = Ticker.Current + skillInfo.StealPeriod;

                foreach (var other in game.Players.Values)
                {
                    if (other.CannotBeDamaged(initiator.Team))
                    {
                        continue;
                    }

                    ref var t = ref initiator.Transform;
                    if (other.Transform.IsInsideCylinder(t.Position.X, t.Position.Y, t.Position.Z, skillInfo.AoE.RadiusSqr, skillInfo.AoE.Height))
                    {
                        var stolen = Math.Min(initiator.Health.MaxHealth * skillInfo.StealPercent, other.Health.Health);

                        initiator.Heals.Add(new HealInfo(HealType.Lifesteal, stolen));

                        other.Damages.Add(
                            new DamageInfo(
                                initiator.Id,
                                new EntityRef(EntityType.LifestealSkill, skill.Id.Item1),
                                DamageType.Pure,
                                stolen));
                    }
                }
            }
        }
    }
}