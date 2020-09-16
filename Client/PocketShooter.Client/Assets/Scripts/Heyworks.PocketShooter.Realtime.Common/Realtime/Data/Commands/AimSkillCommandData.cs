namespace Heyworks.PocketShooter.Realtime.Data
{
    /// <summary>
    /// Skill command data.
    /// </summary>
    public sealed class AimSkillCommandData : IGameCommandData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AimSkillCommandData"/> class.
        /// </summary>
        /// <param name="playerId">playerId.</param>
        /// <param name="skillName">skillName.</param>
        /// <param name="aiming">aiming.</param>
        public AimSkillCommandData(EntityId playerId, SkillName skillName, bool aiming)
        {
            ActorId = playerId;
            SkillName = skillName;
            IsAiming = aiming;
        }

        /// <summary>
        /// Gets player owning the command.
        /// </summary>
        public EntityId ActorId { get; }

        public SimulationDataCode Code => SimulationDataCode.AimSkill;

        /// <summary>
        /// Gets the name of the skill.
        /// </summary>
        /// <value>The name of the skill.</value>
        public SkillName SkillName { get; }

        /// <summary>
        /// Gets a value indicating whether skill in aiming state.
        /// </summary>
        public bool IsAiming { get; }
    }
}