#pragma warning disable SA1649 // File name should match first type name
using Collections.Pooled;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Microsoft.Extensions.Logging;

namespace Heyworks.PocketShooter.Realtime.Simulation
{
    public struct MovedEvent : IServerEvent
    {
    }

    public struct ExpendablesEvent : IServerEvent
    {
    }

    public struct AttackServerEvent : IServerEvent
    {
        public AttackServerEvent(IReadOnlyPooledList<ShotInfo> shots)
        {
            Shots = shots;
        }

        public IReadOnlyPooledList<ShotInfo> Shots { get; }
    }

    public struct KilledServerEvent : IServerEvent, IActorEvent
    {
        public KilledServerEvent(EntityId playerId, EntityId by, DeathType deathType, Tick atTick, TrooperClass lastClass)
        {
            Killed = playerId;
            ActorId = by;
            DeathType = deathType;
            Tick = atTick;
            LastClass = lastClass;
        }

        /// <summary>
        /// Who did kill.
        /// </summary>
        public EntityId ActorId { get; }

        /// <summary>
        /// Who was killed.
        /// </summary>
        public EntityId Killed { get; }

        public TrooperClass LastClass { get; }

        public DeathType DeathType { get; }

        public Tick Tick { get; }

        public override string ToString() => $"{nameof(KilledServerEvent)}{(ActorId, Killed, DeathType, Tick)}";
    }

    public struct DamagedServerEvent : IServerEvent, IActorEvent
    {
        public DamagedServerEvent(EntityId damagedEntity, IReadOnlyPooledList<DamageInfo> damages, Tick atTick)
        {
            ActorId = damagedEntity;
            Damages = damages;
            Tick = atTick;
        }

        // damaged entity
        public EntityId ActorId { get; }
        public IReadOnlyPooledList<DamageInfo> Damages { get; }
        public Tick Tick { get; }

        public override string ToString() => $"{nameof(DamagedServerEvent)}{(ActorId, Tick, Damages.Count)}";
    }

    public struct HealingServerEvent : IServerEvent
    {
        public HealingServerEvent(IReadOnlyPooledList<HealInfo> heals)
        {
            Heals = heals;
        }

        public IReadOnlyPooledList<HealInfo> Heals { get; }
    }

    public struct MedKitUsedEvent : IClientEvent, IServerEvent
    {
        public MedKitUsedEvent(bool healed)
        {
            Healed = healed;
        }

        public bool Healed { get; }
    }

    public struct HealChangeEvent : IClientEvent, IServerEvent
    {
        public HealChangeEvent(bool healed)
        {
            Healed = healed;
        }

        public bool Healed { get; }
    }

    public struct StunChangeEvent : IServerEvent, IClientEvent
    {
        public StunChangeEvent(bool stunned)
        {
            Stunned = stunned;
        }

        public bool Stunned { get; }
    }

    public struct RootChangeEvent : IServerEvent, IClientEvent
    {
        public RootChangeEvent(bool rooted)
        {
            Rooted = rooted;
        }

        public bool Rooted { get; }
    }

    public struct LuckyChangeEvent : IServerEvent, IClientEvent
    {
        public LuckyChangeEvent(bool lucky)
        {
            Lucky = lucky;
        }

        public bool Lucky { get; }
    }

    public struct InvisibilityChangeEvent : IServerEvent, IClientEvent
    {
        public InvisibilityChangeEvent(bool isInvisible)
        {
            IsInvisible = isInvisible;
        }

        public bool IsInvisible { get; }
    }

    public struct DashChangeEvent : IServerEvent, IClientEvent
    {
        public DashChangeEvent(bool isDashing)
        {
            IsDashing = isDashing;
        }

        public bool IsDashing { get; }
    }

    public struct ImmortalityChangeEvent : IServerEvent, IActorEvent
    {
        public ImmortalityChangeEvent(EntityId actorId, bool isImmortal, Tick atTick)
        {            
            ActorId = actorId;
            IsImmortal = isImmortal;
            Tick = atTick;
        }

        // who

        public EntityId ActorId {get;}

        // what
        public bool IsImmortal { get; }

        // when
        public Tick Tick { get; }        

        public override string ToString() => $"{nameof(ImmortalityChangeEvent)}{(ActorId, IsImmortal, Tick)}";
    }

    public struct RageChangedEvent : IServerEvent
    {
        public RageChangedEvent(int progress)
        {
            Progress = progress;
        }

        public int Progress { get; }
    }

    public struct SkillCooldownEvent : IClientEvent
    {
        public SkillCooldownEvent(ushort remaining)
        {
            Remaining = remaining;
        }

        public ushort Remaining { get; }
    }

    public struct SkillStateChangedEvent : IClientEvent, IServerEvent, IActorEvent
    {
        public SkillStateChangedEvent(EntityId actorId, SkillName skillName, SkillState previous, SkillState next, Tick atTick)
        {
            ActorId = actorId;
            SkillName = skillName;
            Next = next;
            Previous = previous;
            Tick = atTick;
        }

        public EntityId ActorId {get;}

        public SkillName SkillName { get; }

        public SkillState Previous { get; }

        public SkillState Next { get; }

        // DEBT: as of now reload and active are mixed leading to issues like
        // https://heyworks.atlassian.net/browse/PSH-602
        // will get issues if cooldown is longer than active time
        public bool Activated => Previous == SkillState.Default
                                 && (Next == SkillState.Active || Next == SkillState.Reloading);

        public Tick Tick { get; }          

        public override string ToString() => $"{typeof(SkillStateChangedEvent)}{(ActorId,SkillName, Previous, Next, Activated, Tick)}";
    }

    public struct SkillAimChangeEvent : IClientEvent, IServerEvent
    {
        public SkillAimChangeEvent(SkillName skillName, bool isAiming)
        {
            SkillName = skillName;
            IsAiming = isAiming;
        }

        public SkillName SkillName { get; }

        public bool IsAiming { get; }
    }

    public struct SkillCastChangedEvent : IClientEvent, IServerEvent
    {
        public SkillCastChangedEvent(SkillName skillName, bool isCasting)
        {
            SkillName = skillName;
            IsCasting = isCasting;
        }

        public SkillName SkillName { get; }

        public bool IsCasting { get; }
    }
}
#pragma warning restore SA1649 // File name should match first type name