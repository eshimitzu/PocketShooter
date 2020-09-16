using System;
using Heyworks.PocketShooter.Realtime.Simulation;

namespace Heyworks.PocketShooter.Realtime.Entities
{
    public interface IPlayerEvents
    {
        EntityId Id { get; }

        IObservable<HealingServerEvent> Healed { get; }

        IObservable<SkillStateChangedEvent> SkillStateChanged { get; }

        IObservable<SkillCooldownEvent> SkillCoolDownChanged { get; }

        IObservable<SkillAimChangeEvent> SkillAimChanged { get; }

        IObservable<SkillCastChangedEvent> SkillCastChanged { get; }

        IObservable<EntityId> ExpendablesChanged { get; }

        IObservable<DamagedServerEvent> Damaged { get; }

        IObservable<KilledServerEvent> Killed { get; }

        IObservable<WeaponStateChangedEvent> WeaponStateChanged { get; }

        IObservable<float> WarmingUpProgressChanged { get; }

        IObservable<bool> StunChanged { get; }

        IObservable<bool> RootChanged { get; }

        IObservable<bool> InvisibleChanged { get; }

        IObservable<ImmortalityChangeEvent> ImmortalityChanged { get; }

        IObservable<int> RageChanged { get; }

        IObservable<bool> LuckyChanged { get; }

        IObservable<bool> DashChanged { get; }

        IObservable<bool> MedKitUsed { get; }

        IObservable<bool> HealChanged { get; }
    }
}