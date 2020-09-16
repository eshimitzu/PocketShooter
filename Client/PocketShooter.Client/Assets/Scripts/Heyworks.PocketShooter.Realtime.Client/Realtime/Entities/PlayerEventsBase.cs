using System;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Simulation;
using UniRx;

namespace Heyworks.PocketShooter.Realtime.Entities
{
    public class PlayerEventsBase : IPlayerEvents
    {
        internal readonly Subject<FpsTransformComponent> Move;

        internal readonly Subject<HealingServerEvent> Heal;

        internal readonly Subject<DamagedServerEvent> Damage = new Subject<DamagedServerEvent>();

        internal readonly Subject<KilledServerEvent> Kill = new Subject<KilledServerEvent>();

        internal readonly Subject<WeaponStateChangedEvent> WeaponStateChange = new Subject<WeaponStateChangedEvent>();

        internal readonly Subject<EntityId> ExpendablesChange = new Subject<EntityId>();

        internal readonly Subject<float> WarmingUpProgressChange = new Subject<float>();

        internal readonly Subject<bool> StunChange = new Subject<bool>();

        internal readonly Subject<bool> RootChange = new Subject<bool>();

        internal readonly Subject<bool> InvisibleChange = new Subject<bool>();

        internal readonly Subject<ImmortalityChangeEvent> ImmortalityChange = new Subject<ImmortalityChangeEvent>();

        internal readonly Subject<bool> LuckyChange = new Subject<bool>();

        internal readonly Subject<int> RageChange = new Subject<int>();

        internal readonly Subject<bool> DashChange = new Subject<bool>();

        internal readonly Subject<bool> MedKitUse = new Subject<bool>();

        internal readonly Subject<bool> HealChange = new Subject<bool>();

        internal readonly Subject<SkillStateChangedEvent> SkillStateChange = new Subject<SkillStateChangedEvent>();

        internal readonly Subject<SkillCooldownEvent> SkillCoolDownChange = new Subject<SkillCooldownEvent>();

        internal readonly Subject<SkillAimChangeEvent> SkillAimChange = new Subject<SkillAimChangeEvent>();

        internal readonly Subject<SkillCastChangedEvent> SkillCastChange = new Subject<SkillCastChangedEvent>();

        public IObservable<EntityId> ExpendablesChanged => ExpendablesChange;

        public IObservable<DamagedServerEvent> Damaged => Damage;

        public IObservable<KilledServerEvent> Killed => Kill;

        public IObservable<WeaponStateChangedEvent> WeaponStateChanged => WeaponStateChange;

        public IObservable<HealingServerEvent> Healed => Heal;

        public IObservable<float> WarmingUpProgressChanged => WarmingUpProgressChange;

        public IObservable<bool> StunChanged => StunChange;

        public IObservable<bool> RootChanged => RootChange;

        public IObservable<bool> InvisibleChanged => InvisibleChange;

        public IObservable<ImmortalityChangeEvent> ImmortalityChanged => ImmortalityChange;

        public IObservable<int> RageChanged => RageChange;

        public IObservable<bool> LuckyChanged => LuckyChange;

        public IObservable<bool> DashChanged => DashChange;

        public IObservable<bool> MedKitUsed => MedKitUse;

        public IObservable<bool> HealChanged => HealChange;

        public IObservable<SkillStateChangedEvent> SkillStateChanged => SkillStateChange;

        public IObservable<SkillCooldownEvent> SkillCoolDownChanged => SkillCoolDownChange;

        public IObservable<SkillAimChangeEvent> SkillAimChanged => SkillAimChange;

        public IObservable<SkillCastChangedEvent> SkillCastChanged => SkillCastChange;

        public EntityId Id { get; }

        internal PlayerEventsBase(EntityId id)
        {
            Id = id;
            Move = new Subject<FpsTransformComponent>();
            Heal = new Subject<HealingServerEvent>();
        }
    }
}