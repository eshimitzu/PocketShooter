using System;
using System.Collections.Generic;
using System.Linq;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Microsoft.Extensions.Logging;

namespace Heyworks.PocketShooter.Realtime.Simulation
{
    // TODO: a.dezhurko Rewrite using unsafe code. Ask d.lahoda
    public static class Reconciliation
    {
        public static void Reconcile(in PlayerState serverStateAtServerTick, ref PlayerState clientStateAtClientTick)
        {
            // TODO: a.dezhurko Move events from player state.
            // This data modifies only on server and must be put out of player state.
            clientStateAtClientTick.ServerEvents = serverStateAtServerTick.ServerEvents;
            clientStateAtClientTick.Damages = serverStateAtServerTick.Damages;
            clientStateAtClientTick.Shots = serverStateAtServerTick.Shots;
            clientStateAtClientTick.Heals = serverStateAtServerTick.Heals;
        }

        public static void Reconcile(ref PlayerState serverStateAtServerTick, ref PlayerState clientStateAtServerTick, ClientState<PlayerState> history)
        {
            ReconcilePlayer(in serverStateAtServerTick, in clientStateAtServerTick, history);
            for (byte skillIndex = 0; skillIndex < 5; skillIndex++)
            {
                ref readonly var past = ref GetSkillByIndex(ref clientStateAtServerTick, skillIndex);
                ref readonly var next = ref GetSkillByIndex(ref serverStateAtServerTick, skillIndex);
                ReconcileSkill(in next, in past, history, skillIndex);
            }

            ReconcileWeapon(in serverStateAtServerTick, in clientStateAtServerTick, history);
        }

        private static void ReconcilePlayer(in PlayerState serverStateAtServerTick, in PlayerState clientStateAtServerTick, ClientState<PlayerState> history)
        {
            if (serverStateAtServerTick.Id != clientStateAtServerTick.Id)
            {
                ReconciliationLog.Trace("Reconciling {name} from {oldValue} to {newValue}", "playerId", clientStateAtServerTick.Id, serverStateAtServerTick.Id);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    history[index].Id = serverStateAtServerTick.Id;
                }
            }

            if (serverStateAtServerTick.Team != clientStateAtServerTick.Team)
            {
                ReconciliationLog.Trace("Reconciling {name} from {oldValue} to {newValue}", "Team", clientStateAtServerTick.Team, serverStateAtServerTick.Team);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    history[index].Team = serverStateAtServerTick.Team;
                }
            }

            if (serverStateAtServerTick.TrooperClass != clientStateAtServerTick.TrooperClass)
            {
                ReconciliationLog.Trace("Reconciling {name} from {oldValue} to {newValue}", "TrooperClass", clientStateAtServerTick.TrooperClass, serverStateAtServerTick.TrooperClass);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    history[index].TrooperClass = serverStateAtServerTick.TrooperClass;
                }
            }

            if (Math.Abs(serverStateAtServerTick.Health.Health - clientStateAtServerTick.Health.Health) > 0.001)
            {
                ReconciliationLog.Trace("Reconciling {name} from {oldValue} to {newValue}", "Health", clientStateAtServerTick.Health.Health, serverStateAtServerTick.Health.Health);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    history[index].Health.Health = serverStateAtServerTick.Health.Health;
                }
            }

            if (Math.Abs(serverStateAtServerTick.Armor.Armor - clientStateAtServerTick.Armor.Armor) > 0.001)
            {
                ReconciliationLog.Trace("Reconciling {name} from {oldValue} to {newValue}", "Armor", clientStateAtServerTick.Armor.Armor, serverStateAtServerTick.Armor.Armor);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    history[index].Armor.Armor = serverStateAtServerTick.Armor.Armor;
                }
            }

            if (serverStateAtServerTick.Effects.Stun.Base.IsStunned != clientStateAtServerTick.Effects.Stun.Base.IsStunned)
            {
                ReconciliationLog.Trace("Reconciling {name} from {oldValue} to {newValue}", "IsStunned", clientStateAtServerTick.Effects.Stun.Base.IsStunned, serverStateAtServerTick.Effects.Stun.Base.IsStunned);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    history[index].Effects.Stun.Base.IsStunned = serverStateAtServerTick.Effects.Stun.Base.IsStunned;
                }
            }

            if (serverStateAtServerTick.Effects.Stun.Expire.ExpireAt != clientStateAtServerTick.Effects.Stun.Expire.ExpireAt)
            {
                ReconciliationLog.Trace("Reconciling {name} from {oldValue} to {newValue}", "Stun.Expire.ExpireAt", clientStateAtServerTick.Effects.Stun.Expire.ExpireAt, serverStateAtServerTick.Effects.Stun.Expire.ExpireAt);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    history[index].Effects.Stun.Expire.ExpireAt = serverStateAtServerTick.Effects.Stun.Expire.ExpireAt;
                }
            }

            if (serverStateAtServerTick.Effects.Root.Base.IsRooted != clientStateAtServerTick.Effects.Root.Base.IsRooted)
            {
                ReconciliationLog.Trace("Reconciling {name} from {oldValue} to {newValue}", "Root.Base.IsRooted", clientStateAtServerTick.Effects.Root.Base.IsRooted, serverStateAtServerTick.Effects.Root.Base.IsRooted);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    history[index].Effects.Root.Base.IsRooted = serverStateAtServerTick.Effects.Root.Base.IsRooted;
                }
            }

            if (serverStateAtServerTick.Effects.Root.Expire.ExpireAt != clientStateAtServerTick.Effects.Root.Expire.ExpireAt)
            {
                ReconciliationLog.Trace("Reconciling {name} from {oldValue} to {newValue}", "Root.Expire.ExpireAt", clientStateAtServerTick.Effects.Root.Expire.ExpireAt, serverStateAtServerTick.Effects.Root.Expire.ExpireAt);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    history[index].Effects.Root.Expire.ExpireAt = serverStateAtServerTick.Effects.Root.Expire.ExpireAt;
                }
            }

            if (serverStateAtServerTick.Effects.Invisible.Base.IsInvisible != clientStateAtServerTick.Effects.Invisible.Base.IsInvisible)
            {
                ReconciliationLog.Trace("Reconciling {name} from {oldValue} to {newValue}", "Invisible.Base.IsInvisible", clientStateAtServerTick.Effects.Invisible.Base.IsInvisible, serverStateAtServerTick.Effects.Invisible.Base.IsInvisible);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    history[index].Effects.Invisible.Base.IsInvisible = serverStateAtServerTick.Effects.Invisible.Base.IsInvisible;
                }
            }

            if (serverStateAtServerTick.Effects.Invisible.Expire.ExpireAt != clientStateAtServerTick.Effects.Invisible.Expire.ExpireAt)
            {
                ReconciliationLog.Trace("Reconciling {name} from {oldValue} to {newValue}", "Invisible.Expire.ExpireAt", clientStateAtServerTick.Effects.Invisible.Expire.ExpireAt, serverStateAtServerTick.Effects.Invisible.Expire.ExpireAt);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    history[index].Effects.Invisible.Expire.ExpireAt = serverStateAtServerTick.Effects.Invisible.Expire.ExpireAt;
                }
            }

            if (serverStateAtServerTick.Effects.Immortality.Base.IsImmortal != clientStateAtServerTick.Effects.Immortality.Base.IsImmortal)
            {
                ReconciliationLog.Trace("Reconciling {name} from {oldValue} to {newValue}", "Immortality.Base.IsImmortal", clientStateAtServerTick.Effects.Immortality.Base.IsImmortal, serverStateAtServerTick.Effects.Immortality.Base.IsImmortal);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    history[index].Effects.Immortality.Base.IsImmortal = serverStateAtServerTick.Effects.Immortality.Base.IsImmortal;
                }
            }

            if (serverStateAtServerTick.Effects.Immortality.Expire.ExpireAt != clientStateAtServerTick.Effects.Immortality.Expire.ExpireAt)
            {
                ReconciliationLog.Trace("Reconciling {name} from {oldValue} to {newValue}", "Immortality.Expire.ExpireAt", clientStateAtServerTick.Effects.Immortality.Expire.ExpireAt, serverStateAtServerTick.Effects.Immortality.Expire.ExpireAt);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    history[index].Effects.Immortality.Expire.ExpireAt = serverStateAtServerTick.Effects.Immortality.Expire.ExpireAt;
                }
            }

            if (serverStateAtServerTick.Effects.Lucky.Base.IsLucky != clientStateAtServerTick.Effects.Lucky.Base.IsLucky)
            {
                ReconciliationLog.Trace("Reconciling {name} from {oldValue} to {newValue}", "Lucky.Base.IsLucky", clientStateAtServerTick.Effects.Lucky.Base.IsLucky, serverStateAtServerTick.Effects.Lucky.Base.IsLucky);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    history[index].Effects.Lucky.Base.IsLucky = serverStateAtServerTick.Effects.Lucky.Base.IsLucky;
                }
            }

            if (serverStateAtServerTick.Effects.Lucky.Expire.ExpireAt != clientStateAtServerTick.Effects.Lucky.Expire.ExpireAt)
            {
                ReconciliationLog.Trace("Reconciling {name} from {oldValue} to {newValue}", "Lucky.Expire.ExpireAt", clientStateAtServerTick.Effects.Lucky.Expire.ExpireAt, serverStateAtServerTick.Effects.Lucky.Expire.ExpireAt);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    history[index].Effects.Lucky.Expire.ExpireAt = serverStateAtServerTick.Effects.Lucky.Expire.ExpireAt;
                }
            }

            if (serverStateAtServerTick.Effects.Jump.Base.IsJumping != clientStateAtServerTick.Effects.Jump.Base.IsJumping)
            {
                ReconciliationLog.Trace("Reconciling {name} from {oldValue} to {newValue}", "Jump.Base.IsJumping", clientStateAtServerTick.Effects.Jump.Base.IsJumping, serverStateAtServerTick.Effects.Jump.Base.IsJumping);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    history[index].Effects.Jump.Base.IsJumping = serverStateAtServerTick.Effects.Jump.Base.IsJumping;
                }
            }

            if (serverStateAtServerTick.Effects.Jump.Expire.ExpireAt != clientStateAtServerTick.Effects.Jump.Expire.ExpireAt)
            {
                ReconciliationLog.Trace("Reconciling {name} from {oldValue} to {newValue}", "Jump.Expire.ExpireAt", clientStateAtServerTick.Effects.Jump.Expire.ExpireAt, serverStateAtServerTick.Effects.Jump.Expire.ExpireAt);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    history[index].Effects.Jump.Expire.ExpireAt = serverStateAtServerTick.Effects.Jump.Expire.ExpireAt;
                }
            }

            if (serverStateAtServerTick.Effects.Dash.Base.IsDashing != clientStateAtServerTick.Effects.Dash.Base.IsDashing)
            {
                ReconciliationLog.Trace("Reconciling {name} from {oldValue} to {newValue}", "Dash.Base.IsDashing", clientStateAtServerTick.Effects.Dash.Base.IsDashing, serverStateAtServerTick.Effects.Dash.Base.IsDashing);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    history[index].Effects.Dash.Base.IsDashing = serverStateAtServerTick.Effects.Dash.Base.IsDashing;
                }
            }

            if (serverStateAtServerTick.Effects.Dash.Expire.ExpireAt != clientStateAtServerTick.Effects.Dash.Expire.ExpireAt)
            {
                ReconciliationLog.Trace("Reconciling {name} from {oldValue} to {newValue}", "Dash.Expire.ExpireAt", clientStateAtServerTick.Effects.Dash.Expire.ExpireAt, serverStateAtServerTick.Effects.Dash.Expire.ExpireAt);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    history[index].Effects.Dash.Expire.ExpireAt = serverStateAtServerTick.Effects.Dash.Expire.ExpireAt;
                }
            }

            if (serverStateAtServerTick.Effects.Rage.Base.AdditionalDamagePercent != clientStateAtServerTick.Effects.Rage.Base.AdditionalDamagePercent)
            {
                ReconciliationLog.Trace("Reconciling {name} from {oldValue} to {newValue}", "Rage.Base.AdditionalDamagePercent", clientStateAtServerTick.Effects.Rage.Base.AdditionalDamagePercent, serverStateAtServerTick.Effects.Rage.Base.AdditionalDamagePercent);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    history[index].Effects.Rage.Base.AdditionalDamagePercent = serverStateAtServerTick.Effects.Rage.Base.AdditionalDamagePercent;
                }
            }

            if (serverStateAtServerTick.Effects.Rage.Expire.IncreaseDamageAt != clientStateAtServerTick.Effects.Rage.Expire.IncreaseDamageAt)
            {
                ReconciliationLog.Trace("Reconciling {name} from {oldValue} to {newValue}", "Rage.Expire.IncreaseDamageAt", clientStateAtServerTick.Effects.Rage.Expire.IncreaseDamageAt, serverStateAtServerTick.Effects.Rage.Expire.IncreaseDamageAt);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    history[index].Effects.Rage.Expire.IncreaseDamageAt = serverStateAtServerTick.Effects.Rage.Expire.IncreaseDamageAt;
                }
            }

            if (serverStateAtServerTick.Effects.Rage.Expire.DecreaseDamageAt != clientStateAtServerTick.Effects.Rage.Expire.DecreaseDamageAt)
            {
                ReconciliationLog.Trace("Reconciling {name} from {oldValue} to {newValue}", "Rage.Expire.DecreaseDamageAt", clientStateAtServerTick.Effects.Rage.Expire.DecreaseDamageAt, serverStateAtServerTick.Effects.Rage.Expire.DecreaseDamageAt);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    history[index].Effects.Rage.Expire.DecreaseDamageAt = serverStateAtServerTick.Effects.Rage.Expire.DecreaseDamageAt;
                }
            }

            if (serverStateAtServerTick.Effects.Rage.Expire.LastWeaponState != clientStateAtServerTick.Effects.Rage.Expire.LastWeaponState)
            {
                ReconciliationLog.Trace("Reconciling {name} from {oldValue} to {newValue}", "Rage.Expire.LastWeaponState", clientStateAtServerTick.Effects.Rage.Expire.LastWeaponState, serverStateAtServerTick.Effects.Rage.Expire.LastWeaponState);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    history[index].Effects.Rage.Expire.LastWeaponState = serverStateAtServerTick.Effects.Rage.Expire.LastWeaponState;
                }
            }

            if (serverStateAtServerTick.ServerEvents.LastKiller != clientStateAtServerTick.ServerEvents.LastKiller)
            {
                ReconciliationLog.Trace("Reconciling {name} from {oldValue} to {newValue}", "serverStateAtServerTick.ServerEvents.LastKiller", serverStateAtServerTick.ServerEvents.LastKiller, clientStateAtServerTick.ServerEvents.LastKiller);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    history[index].ServerEvents.LastKiller = serverStateAtServerTick.ServerEvents.LastKiller;
                }
            }

            if (serverStateAtServerTick.Effects.MedKit.Base.IsHealing != clientStateAtServerTick.Effects.MedKit.Base.IsHealing)
            {
                ReconciliationLog.Trace("Reconciling medkit {name} from {oldValue} to {newValue}", "State.IsHealing", clientStateAtServerTick.Effects.MedKit.Base.IsHealing, serverStateAtServerTick.Effects.MedKit.Base.IsHealing);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    history[index].Effects.MedKit.Base.IsHealing = serverStateAtServerTick.Effects.MedKit.Base.IsHealing;
                }
            }

            if (serverStateAtServerTick.Effects.MedKit.Owned.NextHealAt != clientStateAtServerTick.Effects.MedKit.Owned.NextHealAt)
            {
                ReconciliationLog.Trace("Reconciling medkit {name} from {oldValue} to {newValue}", "Owned.NextHealAt", clientStateAtServerTick.Effects.MedKit.Owned.NextHealAt, serverStateAtServerTick.Effects.MedKit.Owned.NextHealAt);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    history[index].Effects.MedKit.Owned.NextHealAt = serverStateAtServerTick.Effects.MedKit.Owned.NextHealAt;
                }
            }

            if (serverStateAtServerTick.Effects.MedKit.Owned.ExpiredAt != clientStateAtServerTick.Effects.MedKit.Owned.ExpiredAt)
            {
                ReconciliationLog.Trace("Reconciling medkit {name} from {oldValue} to {newValue}", "Owned.FinishedAt", clientStateAtServerTick.Effects.MedKit.Owned.ExpiredAt, serverStateAtServerTick.Effects.MedKit.Owned.ExpiredAt);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    history[index].Effects.MedKit.Owned.ExpiredAt = serverStateAtServerTick.Effects.MedKit.Owned.ExpiredAt;
                }
            }

            if (serverStateAtServerTick.Effects.Heal.Base.IsHealing != clientStateAtServerTick.Effects.Heal.Base.IsHealing)
            {
                ReconciliationLog.Trace("Reconciling Heal {name} from {oldValue} to {newValue}", "State.IsHealing", clientStateAtServerTick.Effects.Heal.Base.IsHealing, serverStateAtServerTick.Effects.Heal.Base.IsHealing);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    history[index].Effects.Heal.Base.IsHealing = serverStateAtServerTick.Effects.Heal.Base.IsHealing;
                }
            }

            if (serverStateAtServerTick.Effects.Heal.Owned.NextHealAt != clientStateAtServerTick.Effects.Heal.Owned.NextHealAt)
            {
                ReconciliationLog.Trace("Reconciling Heal {name} from {oldValue} to {newValue}", "Owned.NextHealAt", clientStateAtServerTick.Effects.Heal.Owned.NextHealAt, serverStateAtServerTick.Effects.Heal.Owned.NextHealAt);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    history[index].Effects.Heal.Owned.NextHealAt = serverStateAtServerTick.Effects.Heal.Owned.NextHealAt;
                }
            }

            if (serverStateAtServerTick.Effects.Heal.Owned.ExpiredAt != clientStateAtServerTick.Effects.Heal.Owned.ExpiredAt)
            {
                ReconciliationLog.Trace("Reconciling Heal {name} from {oldValue} to {newValue}", "Owned.FinishedAt", clientStateAtServerTick.Effects.Heal.Owned.ExpiredAt, serverStateAtServerTick.Effects.Heal.Owned.ExpiredAt);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    history[index].Effects.Heal.Owned.ExpiredAt = serverStateAtServerTick.Effects.Heal.Owned.ExpiredAt;
                }
            }
        }

        private static void ReconcileSkill(
            in SkillComponents serverStateAtServerTick,
            in SkillComponents clientStateAtServerTick,
            ClientState<PlayerState> history,
            byte skillIndex)
        {
            if (clientStateAtServerTick.Base.State != serverStateAtServerTick.Base.State)
            {
                ReconciliationLog.Trace("Reconciling skill {name} from {oldValue} to {newValue}", "Base.State", clientStateAtServerTick.Base.State, serverStateAtServerTick.Base.State);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    ref var h = ref GetSkillByIndex(ref history[index], skillIndex);
                    h.Base.State = serverStateAtServerTick.Base.State;
                }
            }

            if (clientStateAtServerTick.Base.Name != serverStateAtServerTick.Base.Name)
            {
                ReconciliationLog.Trace("Reconciling skill {name} from {oldValue} to {newValue}", "Base.Name", clientStateAtServerTick.Base.Name, serverStateAtServerTick.Base.Name);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    ref var h = ref GetSkillByIndex(ref history[index], skillIndex);
                    h.Base.Name = serverStateAtServerTick.Base.Name;
                }
            }

            if (Math.Abs(clientStateAtServerTick.AttackModifier.AdditionalDamage - serverStateAtServerTick.AttackModifier.AdditionalDamage) > 0.001)
            {
                ReconciliationLog.Trace("Reconciling skill {name} from {oldValue} to {newValue}", "AttackModifier.AdditionalDamage", clientStateAtServerTick.AttackModifier.AdditionalDamage, serverStateAtServerTick.AttackModifier.AdditionalDamage);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    ref var h = ref GetSkillByIndex(ref history[index], skillIndex);
                    h.AttackModifier.AdditionalDamage = serverStateAtServerTick.AttackModifier.AdditionalDamage;
                }
            }

            if (Math.Abs(clientStateAtServerTick.AttackModifier.DamageMultiplier - serverStateAtServerTick.AttackModifier.DamageMultiplier) > 0.001)
            {
                ReconciliationLog.Trace("Reconciling skill {name} from {oldValue} to {newValue}", "AttackModifier.DamageMultiplier", clientStateAtServerTick.AttackModifier.DamageMultiplier, serverStateAtServerTick.AttackModifier.DamageMultiplier);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    ref var h = ref GetSkillByIndex(ref history[index], skillIndex);
                    h.AttackModifier.DamageMultiplier = serverStateAtServerTick.AttackModifier.DamageMultiplier;
                }
            }

            if (clientStateAtServerTick.Lifesteal.NextStealTime != serverStateAtServerTick.Lifesteal.NextStealTime)
            {
                ReconciliationLog.Trace("Reconciling skill {name} from {oldValue} to {newValue}", "Lifesteal.NextStealTime", clientStateAtServerTick.Lifesteal.NextStealTime, serverStateAtServerTick.Lifesteal.NextStealTime);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    ref var h = ref GetSkillByIndex(ref history[index], skillIndex);
                    h.Lifesteal.NextStealTime = serverStateAtServerTick.Lifesteal.NextStealTime;
                }
            }

            if (Math.Abs(clientStateAtServerTick.Lifesteal.Radius - serverStateAtServerTick.Lifesteal.Radius) > 0.01)
            {
                ReconciliationLog.Trace("Reconciling skill {name} from {oldValue} to {newValue}", "Lifesteal.Radius", clientStateAtServerTick.Lifesteal.Radius, serverStateAtServerTick.Lifesteal.Radius);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    ref var h = ref GetSkillByIndex(ref history[index], skillIndex);
                    h.Lifesteal.Radius = serverStateAtServerTick.Lifesteal.Radius;
                }
            }

            if (clientStateAtServerTick.Aiming.Aiming != serverStateAtServerTick.Aiming.Aiming)
            {
                ReconciliationLog.Trace("Reconciling skill {name} from {oldValue} to {newValue}", "Aiming.Aiming", clientStateAtServerTick.Aiming.Aiming, serverStateAtServerTick.Aiming.Aiming);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    ref var h = ref GetSkillByIndex(ref history[index], skillIndex);
                    h.Aiming.Aiming = serverStateAtServerTick.Aiming.Aiming;
                }
            }

            if (clientStateAtServerTick.Casting.Casting != serverStateAtServerTick.Casting.Casting)
            {
                ReconciliationLog.Trace("Reconciling skill {name} from {oldValue} to {newValue}", "Casting.Casting", clientStateAtServerTick.Casting.Casting, serverStateAtServerTick.Casting.Casting);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    ref var h = ref GetSkillByIndex(ref history[index], skillIndex);
                    h.Casting.Casting = serverStateAtServerTick.Casting.Casting;
                }
            }

            if (clientStateAtServerTick.CastingExpire.CastingExpireAt != serverStateAtServerTick.CastingExpire.CastingExpireAt)
            {
                ReconciliationLog.Trace("Reconciling skill {name} from {oldValue} to {newValue}", "CastingExpire.CastingExpireAt", clientStateAtServerTick.CastingExpire.CastingExpireAt, serverStateAtServerTick.CastingExpire.CastingExpireAt);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    ref var h = ref GetSkillByIndex(ref history[index], skillIndex);
                    h.CastingExpire.CastingExpireAt = serverStateAtServerTick.CastingExpire.CastingExpireAt;
                }
            }

            if (clientStateAtServerTick.Consumable.UseCount != serverStateAtServerTick.Consumable.UseCount)
            {
                ReconciliationLog.Trace("Reconciling skill {name} from {oldValue} to {newValue}", "Consumable.UseCount", clientStateAtServerTick.Consumable.UseCount, serverStateAtServerTick.Consumable.UseCount);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    ref var h = ref GetSkillByIndex(ref history[index], skillIndex);
                    h.Consumable.UseCount = serverStateAtServerTick.Consumable.UseCount;
                }
            }

            if (clientStateAtServerTick.Consumable.AvailableCount != serverStateAtServerTick.Consumable.AvailableCount)
            {
                ReconciliationLog.Trace("Reconciling skill {name} from {oldValue} to {newValue}", "Consumable.AvailableCount", clientStateAtServerTick.Consumable.AvailableCount, serverStateAtServerTick.Consumable.AvailableCount);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    ref var h = ref GetSkillByIndex(ref history[index], skillIndex);
                    h.Consumable.AvailableCount = serverStateAtServerTick.Consumable.AvailableCount;
                }
            }

            if (clientStateAtServerTick.StateExpire.ExpireAt != serverStateAtServerTick.StateExpire.ExpireAt)
            {
                ReconciliationLog.Trace("Reconciling skill {name} from {oldValue} to {newValue}", "StateExpire.ExpireAt", clientStateAtServerTick.StateExpire.ExpireAt, serverStateAtServerTick.StateExpire.ExpireAt);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    ref var h = ref GetSkillByIndex(ref history[index], skillIndex);
                    h.StateExpire.ExpireAt = serverStateAtServerTick.StateExpire.ExpireAt;
                }
            }
        }

        private static ref SkillComponents GetSkillByIndex(ref PlayerState stateRef, int index)
        {
            switch (index)
            {
                case 0:
                    return ref stateRef.Skill1;
                case 1:
                    return ref stateRef.Skill2;
                case 2:
                    return ref stateRef.Skill3;
                case 3:
                    return ref stateRef.Skill4;
                case 4:
                    return ref stateRef.Skill5;
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        private static void ReconcileWeapon(in PlayerState serverStateAtServerTick, in PlayerState clientStateAtServerTick, ClientState<PlayerState> history)
        {
            if (serverStateAtServerTick.Weapon.Base.State != clientStateAtServerTick.Weapon.Base.State)
            {
                ReconciliationLog.Trace("Reconciling weapon {name} from {oldValue} to {newValue}", "Base.State", clientStateAtServerTick.Weapon.Base.State, serverStateAtServerTick.Weapon.Base.State);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    history[index].Weapon.Base.State = serverStateAtServerTick.Weapon.Base.State;
                }
            }

            if (serverStateAtServerTick.Weapon.Base.Name != clientStateAtServerTick.Weapon.Base.Name)
            {
                ReconciliationLog.Trace("Reconciling weapon {name} from {oldValue} to {newValue}", "Base.Type", clientStateAtServerTick.Weapon.Base.Name, serverStateAtServerTick.Weapon.Base.Name);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    history[index].Weapon.Base.Name = serverStateAtServerTick.Weapon.Base.Name;
                }
            }

            if (serverStateAtServerTick.Weapon.Consumable.AmmoInClip != clientStateAtServerTick.Weapon.Consumable.AmmoInClip)
            {
                ReconciliationLog.Trace("Reconciling weapon {name} from {oldValue} to {newValue}", "Consumable.AmmoInClip", clientStateAtServerTick.Weapon.Consumable.AmmoInClip, serverStateAtServerTick.Weapon.Consumable.AmmoInClip);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    history[index].Weapon.Consumable.AmmoInClip = serverStateAtServerTick.Weapon.Consumable.AmmoInClip;
                }
            }

            if (serverStateAtServerTick.Weapon.StateExpire.ExpireAt != clientStateAtServerTick.Weapon.StateExpire.ExpireAt)
            {
                ReconciliationLog.Trace("Reconciling weapon {name} from {oldValue} to {newValue}", "StateExpire.ExpireAt", clientStateAtServerTick.Weapon.StateExpire.ExpireAt, serverStateAtServerTick.Weapon.StateExpire.ExpireAt);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    history[index].Weapon.StateExpire.ExpireAt = serverStateAtServerTick.Weapon.StateExpire.ExpireAt;
                }
            }

            if (Math.Abs(serverStateAtServerTick.Weapon.Warmup.Progress - clientStateAtServerTick.Weapon.Warmup.Progress) > 0.001)
            {
                ReconciliationLog.Trace(
                    "Reconciling weapon {name} from {oldValue} to {newValue} diff {diff}",
                    "Warmup.Progress",
                    clientStateAtServerTick.Weapon.Warmup.Progress,
                    serverStateAtServerTick.Weapon.Warmup.Progress,
                    clientStateAtServerTick.Weapon.Warmup.Progress - serverStateAtServerTick.Weapon.Warmup.Progress);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    history[index].Weapon.Warmup.Progress = serverStateAtServerTick.Weapon.Warmup.Progress;
                }
            }

            if (serverStateAtServerTick.Weapon.Warmup.State != clientStateAtServerTick.Weapon.Warmup.State)
            {
                ReconciliationLog.Trace("Reconciling weapon {name} from {oldValue} to {newValue}", "Warmup.State", clientStateAtServerTick.Weapon.Warmup.State, serverStateAtServerTick.Weapon.Warmup.State);
                for (var index = history.Server; index <= history.Client; index++)
                {
                    history[index].Weapon.Warmup.State = serverStateAtServerTick.Weapon.Warmup.State;
                }
            }
        }
    }
}