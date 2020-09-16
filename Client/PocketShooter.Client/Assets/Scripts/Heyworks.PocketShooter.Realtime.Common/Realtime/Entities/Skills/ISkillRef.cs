using System.Collections.Generic;
using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Entities.Skills
{
    /// <summary>
    /// Skill reference.
    /// </summary>
    public interface ISkillRef : IRef<SkillComponents>
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        (EntityId PlayerId, SkillName SkillName) Id { get; }

        /// <summary>
        /// Apply state.
        /// </summary>
        /// <param name="playerStateRef">player ref.</param>
        void ApplyState(IRef<PlayerState> playerStateRef);
    }
}