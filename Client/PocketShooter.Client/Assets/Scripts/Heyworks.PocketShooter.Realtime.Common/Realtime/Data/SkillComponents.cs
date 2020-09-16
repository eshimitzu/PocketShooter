namespace Heyworks.PocketShooter.Realtime.Data
{
    /// <summary>
    /// Skill components.
    /// </summary>
    public struct SkillComponents
    {
        /// <summary>
        /// Base skill component.
        /// </summary>
        public SkillBaseComponent Base;

        /// <summary>
        /// The timestamp when skill state should expired.
        /// </summary>
        public SkillStateExpireComponent StateExpire;

        /// <summary>
        /// The attack modifier for all.
        /// </summary>
        public SkillAttackModifierComponent AttackModifier;

        /// <summary>
        /// Skill lifesteal component.
        /// </summary>
        public LifestealBaseComponent Lifesteal;

        /// <summary>
        /// Skill aiming component.
        /// </summary>
        public SkillAimingComponent Aiming;

        /// <summary>
        /// Skill casting component.
        /// </summary>
        public SkillCastingComponent Casting;

        /// <summary>
        /// Timestamp when casting should stop.
        /// </summary>
        public SkillCastingExpireComponent CastingExpire;

        /// <summary>
        /// The Consumable.
        /// </summary>
        public SkillConsumableComponent Consumable;
    }

    /// <summary>
    /// For all.
    /// </summary>
    public struct SkillBaseComponent : IForAll
    {
        /// <summary>
        /// Current skill name.
        /// </summary>
        public SkillName Name;
                
        /// <summary>
        /// Current skill state.
        /// </summary>
        public SkillState State;

        public override string ToString() => $"{nameof(SkillBaseComponent)}{(Name, State)}";
    }

    /// <summary>
    /// Attack modifier.
    /// </summary>
    public struct SkillAttackModifierComponent : IForAll
    {
        /// <summary>
        /// The damage multiplier.
        /// </summary>
        public float DamageMultiplier;

        /// <summary>
        /// The additional damage.
        /// </summary>
        public float AdditionalDamage;
    }

    /// <summary>
    /// Component for skill state expiration time.
    /// </summary>
    public struct SkillStateExpireComponent : IForOwner
    {
        /// <summary>
        /// Timestamp when skill state should expired.
        /// </summary>
        public int ExpireAt;
    }

    /// <summary>
    /// Lifesteal skill Component.
    /// </summary>
    public struct LifestealBaseComponent : IForOwner // IForServer
    {
        /// <summary>
        /// Time of the next lifesteal action.
        /// </summary>
        public int NextStealTime;

        /// <summary>
        /// Radius of lifesteal zone.
        /// </summary>
        public float Radius;
    }

    /// <summary>
    /// Component for skills with player-aiming logic.
    /// </summary>
    public struct SkillAimingComponent : IForAll
    {
        /// <summary>
        /// Is player in skill-aiming state.
        /// </summary>
        public bool Aiming;
    }

    /// <summary>
    /// Component for skills with casting time.
    /// </summary>
    public struct SkillCastingComponent : IForAll
    {
        /// <summary>
        /// Is the skill casting or not.
        /// </summary>
        public bool Casting;
    }

    /// <summary>
    /// Component with timestamp for casting end.
    /// </summary>
    public struct SkillCastingExpireComponent : IForOwner
    {
        /// <summary>
        /// Timestamp when skill casting ends and skill used.
        /// </summary>
        public int CastingExpireAt;
    }

    /// <summary>
    /// SkillAnimationComponent.
    /// </summary>
    public struct SkillConsumableComponent : IForOwner
    {
        /// <summary>
        /// Number of skill uses.
        /// </summary>
        public int UseCount;

        /// <summary>
        /// Available count.
        /// </summary>
        public int AvailableCount;
    }
}