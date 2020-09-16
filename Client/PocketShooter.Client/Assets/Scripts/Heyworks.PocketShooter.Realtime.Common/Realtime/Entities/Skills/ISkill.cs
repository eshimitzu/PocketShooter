using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Entities.Skills
{
    /// <summary>
    /// Skill.
    /// </summary>
    public interface ISkill : IEntity<(EntityId, SkillName)>
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        SkillName Name { get; }

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <value>The state.</value>
        SkillState State { get; }
    }
}
