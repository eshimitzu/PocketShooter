using Collections.Pooled;
using Heyworks.Realtime.Serialization;

namespace Heyworks.PocketShooter.Realtime.Data
{
    public class ApplyAoECommandData : IGameCommandData
    {
        public ApplyAoECommandData(EntityId actorId, SkillName skill, PooledList<EntityId> victims)
        {
            ActorId = actorId;
            Skill = skill;
            Victims = victims;
        }

        /// <summary>
        /// Gets player owning the command.
        /// </summary>
        public EntityId ActorId { get; }

        public SimulationDataCode Code => SimulationDataCode.UseAoESkill;

        public PooledList<EntityId> Victims { get; }

        [Limit(typeof(SkillName))]
        public SkillName Skill { get; }
    }
}
