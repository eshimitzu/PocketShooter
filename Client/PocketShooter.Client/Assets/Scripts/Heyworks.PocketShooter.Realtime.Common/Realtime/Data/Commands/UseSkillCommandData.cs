namespace Heyworks.PocketShooter.Realtime.Data
{
    /// <summary>
    /// Skill command data.
    /// </summary>
    public sealed class UseSkillCommandData : IGameCommandData
    {
        public UseSkillCommandData(EntityId playerId, SkillName skillName)
        {
            ActorId = playerId;
            SkillName = skillName;
        }

        /// <summary>
        /// Gets player owning the command.
        /// </summary>
        public EntityId ActorId { get; }

        public SimulationDataCode Code => SimulationDataCode.UseSkill;

        /// <summary>
        /// Gets the name of the skill.
        /// </summary>
        /// <value>The name of the skill.</value>
        public SkillName SkillName { get; }
    }
}