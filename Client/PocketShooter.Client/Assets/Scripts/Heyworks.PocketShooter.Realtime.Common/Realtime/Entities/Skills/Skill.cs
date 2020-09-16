using System.Collections.Generic;
using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Entities.Skills
{
    /// <summary>
    /// Skill.
    /// </summary>
    public class Skill : ISkill, ISkillForSystem
    {
        public (EntityId, SkillName) Id => (SkillRef.Id);

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public SkillName Name { get; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>The state.</value>
        public SkillState State
        {
            get => SkillRef.Value.Base.State;
            set => SkillRef.Value.Base.State = value;
        }

        /// <summary>
        /// Gets the state of the skill.
        /// </summary>
        /// <value>The state of the skill.</value>
        protected ISkillRef SkillRef { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Skill"/> class.
        /// </summary>
        /// <param name="skillRef">Skill reference.</param>
        protected Skill(ISkillRef skillRef)
        {
            Name = skillRef.Value.Base.Name;
            SkillRef = skillRef;
        }

        /// <summary>
        /// Applies the state.
        /// </summary>
        /// <param name="playerStateRef">Player reference.</param>
        public void ApplyState(IRef<PlayerState> playerStateRef)
        {
            SkillRef.ApplyState(playerStateRef);
        }

        /// <summary>
        /// Returns true if the skill can be used now.
        /// </summary>
        public virtual bool CanUseSkill()
        {
            return State == SkillState.Default;
        }
    }
}
