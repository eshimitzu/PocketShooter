using System;
using System.Collections.Generic;
using Collections.Pooled;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities.Skills;
using Heyworks.PocketShooter.Realtime.Entities.Weapon;
using Heyworks.PocketShooter.Realtime.Simulation;
using UniRx;
using UnityEngine;

namespace Heyworks.PocketShooter.Realtime.Entities
{
    public class Game : IClientGame
    {
        private readonly Dictionary<byte, IZone> zones;
        private readonly PlayerInfo playerInfo;
        private readonly EntityId localPlayerId;
        private SimulationRef serverStateAtServerTick;

        public ITicker Ticker { get; private set; }

        public EntityId LocalPlayerId => localPlayerId;

        public ClientPlayer LocalPlayer { get; private set; }

        public Team Team1 { get; private set; }

        public Team Team2 { get; private set; }

        public DominationModeInfo ModeInfo { get; }

        #region Observables

        private Subject<RemotePlayerJoinedEvent> remotePlayerJoined = new Subject<RemotePlayerJoinedEvent>();

        private Subject<RemotePlayerLeavedEvent> remotePlayerLeaved = new Subject<RemotePlayerLeavedEvent>();

        private Subject<ResetWorldEvent> worldCleared = new Subject<ResetWorldEvent>();

        private Subject<RemotePlayerRespawnedEvent> remotePlayerRespawned = new Subject<RemotePlayerRespawnedEvent>();

        private Subject<LocalPlayerSpawnedEvent> localPlayerSpawned = new Subject<LocalPlayerSpawnedEvent>();

        private Subject<LocalPlayerRespawnedEvent> localPlayerRespawned = new Subject<LocalPlayerRespawnedEvent>();

        private Subject<int> stateUpdated = new Subject<int>();

        private PooledDictionary<EntityId, RemotePlayer> currentPlayers;

        public IObservable<RemotePlayerJoinedEvent> RemotePlayerJoined => remotePlayerJoined;

        public IObservable<RemotePlayerLeavedEvent> RemotePlayerLeaved => remotePlayerLeaved;

        public IObservable<ResetWorldEvent> WorldCleared => worldCleared;

        public IObservable<RemotePlayerRespawnedEvent> RemotePlayerRespawned => remotePlayerRespawned;

        public IObservable<LocalPlayerSpawnedEvent> LocalPlayerSpawned => localPlayerSpawned;

        public IObservable<LocalPlayerRespawnedEvent> LocalPlayerRespawned => localPlayerRespawned;

        public IObservable<int> StateUpdated => stateUpdated;

        public IReadOnlyDictionary<byte, IZone> Zones => zones;

        public IReadOnlyDictionary<EntityId, RemotePlayer> Players => currentPlayers;
        #endregion

        public Game(SimulationRef gameStateRef, EntityId localPlayerId, DominationModeInfo modeInfo, PlayerInfo playerInfo, ITicker ticker) // + logger?
        {
            serverStateAtServerTick = gameStateRef;
            this.localPlayerId = localPlayerId;
            ModeInfo = modeInfo;
            this.playerInfo = playerInfo;
            Ticker = ticker;

            currentPlayers = new PooledDictionary<EntityId, RemotePlayer>(ModeInfo.MaxPlayers);
            Team1 = new Team(new Team1Ref(gameStateRef), ModeInfo.Map.Teams[0]);
            Team2 = new Team(new Team2Ref(gameStateRef), ModeInfo.Map.Teams[1]);

            var zonesConfig = ModeInfo.Map.Zones;
            zones = new Dictionary<byte, IZone>(zonesConfig.Length);
            for (byte zoneIndex = 0; zoneIndex < zonesConfig.Length; zoneIndex++)
            {
                var zoneConfig = zonesConfig[zoneIndex];
                zones.Add(zoneConfig.Id, new Zone(new ZoneRef(gameStateRef, zoneIndex), zoneConfig));
            }

            ConsumablesState = new ConsumablesMatchState();
            ConsumablesState.AddPlayer(localPlayerId, new ConsumablesInfo(playerInfo.Consumables.TotalOffensives, playerInfo.Consumables.TotalSupports));
        }

        public IPlayer GetPlayer(EntityId id)
        {
            return currentPlayers.TryGetValue(id, out RemotePlayer player)
                ? player
                : LocalPlayer.Id == id
                    ? LocalPlayer as IPlayer
                    : null;
        }

        public Team GetTeam(TeamNo number)
        {
            switch (number)
            {
                case TeamNo.First:
                    return Team1;
                case TeamNo.Second:
                    return Team2;
                default:
                    throw new ArgumentException($"Team number must be specified. TeamNo = {number.ToString()}");
            }
        }

        public ConsumablesMatchState ConsumablesState { get; }

        /// <summary>
        /// Sets game state to the provided reference.
        /// </summary>
        /// <param name="serverStateAtServerTick">The state reference to be set.</param>        
        /// <param name="events">Simulation events.</param>
        public void SetState(SimulationRef serverStateAtServerTick, ClientState<PlayerState> clientState, SimulationEvents events)
        {
            this.serverStateAtServerTick = serverStateAtServerTick;

            SetTeamsAndZones();

            // will spawn this dictionaries
            var oldWorld = currentPlayers;
            currentPlayers = new PooledDictionary<EntityId, RemotePlayer>(10);
            var players = serverStateAtServerTick.Value.Players.Span;
            for (byte i = 0; i < players.Length; i++)
            {
                var serverPlayerStateAtServerTick = new PlayerRef(this.serverStateAtServerTick, i);

                if (localPlayerId == serverPlayerStateAtServerTick.Id)
                {
                    if (!serverStateAtServerTick.IsInterpolated)
                    {
                        HandleLocalPlayer(events, serverPlayerStateAtServerTick, clientState);
                    }
                }
                else if (oldWorld.TryGetValue(serverPlayerStateAtServerTick.Id, out var remotePlayer))
                {
                    RaiseRemotePlayerEvents(events, remotePlayer, serverPlayerStateAtServerTick, serverStateAtServerTick.Tick);
                }
                else
                {
                    SpawnRemotePlayer(events, serverPlayerStateAtServerTick, serverStateAtServerTick.Tick);
                }
            }

            RaiseLeaved(oldWorld);

            stateUpdated.OnNext(Ticker.Current);
            oldWorld.Dispose();
        }

        /// <summary>
        /// Remove all the world entities(except local player who is always in the future), for example if their states are too old and were overwritten.
        /// </summary>
        public void ClearWorld()
        {
            currentPlayers.Dispose();
            worldCleared.OnNext(default);
        }

        private void HandleLocalPlayer(
            SimulationEvents events, PlayerRef serverStateAtServerTick, ClientState<PlayerState> clientState)
        {
            if (LocalPlayer == null)
            {
                // ISSUE: v.shimkovich
                // No reconciliation needed. Spawn current player and copy new ref to the local player state.
                SpawnLocalPlayer(clientState.AtClientTick);
            }
            else if (LocalPlayer.IsDead && serverStateAtServerTick.Value.Health.Health > 0)
            {
                // No reconciliation needed. Respawn current player and copy new ref to the local player state.
                serverStateAtServerTick.Value.Clone(ref clientState.AtClientTick.Value);
                RespawnLocalPlayer(clientState.AtClientTick);
            }
            else
            {
                RaiseLocalPlayerEvents(events, ref serverStateAtServerTick.Value, ref clientState.AtServerTick.Value, clientState.Server);
                Reconciliation.Reconcile(in serverStateAtServerTick.Value, ref clientState.AtClientTick.Value);
                Reconciliation.Reconcile(ref serverStateAtServerTick.Value, ref clientState.AtServerTick.Value, clientState);
            }
        }

        private void RaiseRemotePlayerEvents(SimulationEvents events, RemotePlayer clientPlayerRef, PlayerRef serverPlayerTickRef, Tick serverTick)
        {
            ref var server = ref serverPlayerTickRef.Value;
            ref var client = ref clientPlayerRef.StateRef.Value;

            // TODO: use stucts in model directly to avoid ad hock copy
            var prevIsAlive = clientPlayerRef.IsAlive;
            var prevMedKit = clientPlayerRef.Effects.MedKit.IsHealing;
            var prevHeal = clientPlayerRef.Effects.Heal.IsHealing;
            var prevWeaponState = clientPlayerRef.CurrentWeapon.State;
            var prevStunned = clientPlayerRef.Effects.Stun.IsStunned;
            var prevRooted = clientPlayerRef.Effects.Root.IsRooted;
            var prevInvisibility = clientPlayerRef.Effects.Invisible.IsInvisible;
            var prevRageProgress = clientPlayerRef.Effects.Rage.AdditionalDamagePercent;
            var prevLucky = clientPlayerRef.Effects.Lucky.IsLucky;
            var prevDash = clientPlayerRef.Effects.Dash.IsDashing;

            var warmupWeapon = clientPlayerRef.CurrentWeapon as IWarmingUpWeapon;
            var prevWarmupProgress = warmupWeapon?.WarmupProgress;

            clientPlayerRef.ApplyState(serverPlayerTickRef);
            clientPlayerRef.Events.Move.OnNext(clientPlayerRef.Transform);

            currentPlayers.Add(server.Id, clientPlayerRef);

            RaiseSkillStateEvents(events, clientPlayerRef.Events, clientPlayerRef.Id, in client.Skill1, in serverPlayerTickRef.Value.Skill1, serverTick);
            RaiseSkillStateEvents(events, clientPlayerRef.Events, clientPlayerRef.Id, in client.Skill2, in serverPlayerTickRef.Value.Skill2, serverTick);
            RaiseSkillStateEvents(events, clientPlayerRef.Events, clientPlayerRef.Id, in client.Skill3, in serverPlayerTickRef.Value.Skill3, serverTick);
            RaiseSkillStateEvents(events, clientPlayerRef.Events, clientPlayerRef.Id, in client.Skill4, in serverPlayerTickRef.Value.Skill4, serverTick);
            RaiseSkillStateEvents(events, clientPlayerRef.Events, clientPlayerRef.Id, in client.Skill5, in serverPlayerTickRef.Value.Skill5, serverTick);

            if (server.Shots.Count > 0)
            {
                events.EnqueueServer((clientPlayerRef.Events, new AttackServerEvent(server.Shots)));
            }

            if (server.Damages.Count > 0)
            {
                events.EnqueueServer((clientPlayerRef.Events, new DamagedServerEvent(clientPlayerRef.Id, server.Damages, serverTick)));
            }

            if (Math.Abs(client.Health.Health - server.Health.Health) > 0.01 || Math.Abs(client.Armor.Armor - server.Armor.Armor) > 0.01)
            {
                events.EnqueueServer((clientPlayerRef.Events, default(ExpendablesEvent)));
            }

            if (prevWeaponState != server.Weapon.Base.State)
            {
                events.EnqueueServer((clientPlayerRef.Events,
                    new WeaponStateChangedEvent(client.Id, prevWeaponState, server.Weapon.Base.State)));
            }

            if (prevWarmupProgress.HasValue && Math.Abs(prevWarmupProgress.Value - server.Weapon.Warmup.Progress) > Mathf.Epsilon)
            {
                events.EnqueueServer((clientPlayerRef.Events,
                    new WarmingUpProgressChangedEvent(server.Weapon.Warmup.Progress)));
            }

            if (!prevIsAlive && server.Health.Health > 0)
            {
                var newRemotePlayer = new RemotePlayer(serverPlayerTickRef);

                newRemotePlayer.ApplyState(serverPlayerTickRef);
                newRemotePlayer.Events.Move.OnNext(newRemotePlayer.Transform);
                currentPlayers.Remove(newRemotePlayer.Id);
                currentPlayers.Add(newRemotePlayer.Id, newRemotePlayer);

                var rse = new RemotePlayerRespawnedEvent(
                    clientPlayerRef,
                    server.TrooperClass,
                    server.Weapon.Base.Name);
                remotePlayerRespawned.OnNext(rse);
            }

            if (prevStunned != server.Effects.Stun.Base.IsStunned)
            {
                events.EnqueueServer((clientPlayerRef.Events,
                    new StunChangeEvent(server.Effects.Stun.Base.IsStunned)));
            }

            if (prevRooted != server.Effects.Root.Base.IsRooted)
            {
                events.EnqueueServer((clientPlayerRef.Events,
                    new RootChangeEvent(server.Effects.Root.Base.IsRooted)));
            }

            if (prevInvisibility != server.Effects.Invisible.Base.IsInvisible)
            {
                events.EnqueueServer((clientPlayerRef.Events,
                    new InvisibilityChangeEvent(server.Effects.Invisible.Base.IsInvisible)));
            }

            if (client.Effects.IsImmortal() != server.Effects.IsImmortal())
            {
                events.EnqueueServer((clientPlayerRef.Events, new ImmortalityChangeEvent(server.Id, server.Effects.IsImmortal(), serverTick)));
            }

            if (prevLucky != server.Effects.Lucky.Base.IsLucky)
            {
                events.EnqueueServer((clientPlayerRef.Events,
                    new LuckyChangeEvent(server.Effects.Lucky.Base.IsLucky)));
            }

            if (Math.Abs(prevRageProgress - server.Effects.Rage.Base.AdditionalDamagePercent) > Mathf.Epsilon)
            {
                events.EnqueueServer((clientPlayerRef.Events,
                    new RageChangedEvent(server.Effects.Rage.Base.AdditionalDamagePercent)));
            }

            if (prevDash != server.Effects.Dash.Base.IsDashing)
            {
                events.EnqueueServer((clientPlayerRef.Events, new DashChangeEvent(server.Effects.Dash.Base.IsDashing)));
            }

            if (prevMedKit != server.Effects.MedKit.Base.IsHealing)
            {
                events.EnqueueServer((clientPlayerRef.Events, new MedKitUsedEvent(true)));
            }

            if (prevHeal != server.Effects.Heal.Base.IsHealing)
            {
                events.EnqueueServer((clientPlayerRef.Events, new HealChangeEvent(true)));
            }

            RaiseHealing(events, clientPlayerRef.Events, ref serverPlayerTickRef.Value);

            RaiseKilled(events, clientPlayerRef.Events, ref client, ref serverPlayerTickRef.Value, serverTick);
        }

        private void RaiseLocalPlayerEvents(SimulationEvents events, ref PlayerState serverStateAtServerTick, ref PlayerState clientStateAtServerTick, Tick serverTick)
        {
            // NOTE: ambient object LocalPlayer is bad smell
            RaiseSkillStateEvents(events, LocalPlayer.Events, serverStateAtServerTick.Id, in clientStateAtServerTick.Skill1, in serverStateAtServerTick.Skill1, serverTick);
            RaiseSkillStateEvents(events, LocalPlayer.Events, serverStateAtServerTick.Id, in clientStateAtServerTick.Skill2, in serverStateAtServerTick.Skill2, serverTick);
            RaiseSkillStateEvents(events, LocalPlayer.Events, serverStateAtServerTick.Id, in clientStateAtServerTick.Skill3, in serverStateAtServerTick.Skill3, serverTick);
            RaiseSkillStateEvents(events, LocalPlayer.Events, serverStateAtServerTick.Id, in clientStateAtServerTick.Skill4, in serverStateAtServerTick.Skill4, serverTick);
            RaiseSkillStateEvents(events, LocalPlayer.Events, serverStateAtServerTick.Id, in clientStateAtServerTick.Skill5, in serverStateAtServerTick.Skill5, serverTick);
            RaiseShots(events, in serverStateAtServerTick);

            if (serverStateAtServerTick.Damages.Count > 0)
            {
                var damageEvent = new DamagedServerEvent(serverStateAtServerTick.Id, serverStateAtServerTick.Damages, serverTick);
                events.EnqueueServer((LocalPlayer.Events, damageEvent));
            }

            if (Math.Abs(clientStateAtServerTick.Health.Health - serverStateAtServerTick.Health.Health) > 0.01
             || Math.Abs(clientStateAtServerTick.Armor.Armor - serverStateAtServerTick.Armor.Armor) > 0.01)
            {
                events.EnqueueServer((LocalPlayer.Events, default(ExpendablesEvent)));
            }

            RaiseWeaponEvents(events, serverStateAtServerTick.Id, in clientStateAtServerTick.Weapon, in serverStateAtServerTick.Weapon);
            RaiseStunEvents(events, in clientStateAtServerTick.Effects.Stun, in serverStateAtServerTick.Effects.Stun);

            if (clientStateAtServerTick.Effects.Root.Base.IsRooted != serverStateAtServerTick.Effects.Root.Base.IsRooted)
            {
                events.EnqueueServer((LocalPlayer.Events,
                    new RootChangeEvent(serverStateAtServerTick.Effects.Root.Base.IsRooted)));
            }

            if (clientStateAtServerTick.Effects.Invisible.Base.IsInvisible != serverStateAtServerTick.Effects.Invisible.Base.IsInvisible)
            {
                events.EnqueueServer((LocalPlayer.Events,
                    new InvisibilityChangeEvent(serverStateAtServerTick.Effects.Invisible.Base.IsInvisible)));
            }

            if (clientStateAtServerTick.Effects.IsImmortal() != serverStateAtServerTick.Effects.IsImmortal())
            {
                events.EnqueueServer((LocalPlayer.Events, new ImmortalityChangeEvent(serverStateAtServerTick.Id, serverStateAtServerTick.Effects.IsImmortal(), serverTick)));
            }

            if (clientStateAtServerTick.Effects.Lucky.Base.IsLucky != serverStateAtServerTick.Effects.Lucky.Base.IsLucky)
            {
                events.EnqueueServer((LocalPlayer.Events,
                    new LuckyChangeEvent(serverStateAtServerTick.Effects.Lucky.Base.IsLucky)));
            }

            if (clientStateAtServerTick.Effects.Dash.Base.IsDashing != serverStateAtServerTick.Effects.Dash.Base.IsDashing)
            {
                events.EnqueueServer(
                    (LocalPlayer.Events, new DashChangeEvent(serverStateAtServerTick.Effects.Dash.Base.IsDashing)));
            }

            RaiseRageDamage(events, in clientStateAtServerTick.Effects.Rage, in serverStateAtServerTick.Effects.Rage);
            RaiseHealing(events, LocalPlayer.Events, ref serverStateAtServerTick);
            RaiseKilled(events, LocalPlayer.Events, ref clientStateAtServerTick, ref serverStateAtServerTick, serverTick);
        }

        private void RaiseShots(SimulationEvents events, in PlayerState serverStateAtServerTick)
        {
            if (serverStateAtServerTick.Shots.Count > 0)
            {
                events.EnqueueServer((LocalPlayer.Events, new AttackServerEvent(serverStateAtServerTick.Shots)));
            }
        }

        private void RaiseRageDamage(SimulationEvents events, in RageComponents clientStateAtServerTick, in RageComponents serverStateAtServerTick)
        {
            var rageDamage = clientStateAtServerTick.Base.AdditionalDamagePercent - serverStateAtServerTick.Base.AdditionalDamagePercent;
            if (Math.Abs(rageDamage) > Mathf.Epsilon)
            {
                events.EnqueueServer((LocalPlayer.Events, new RageChangedEvent(serverStateAtServerTick.Base.AdditionalDamagePercent)));
            }
        }

        private void RaiseStunEvents(SimulationEvents events, in StunComponents clientStateAtServerTick, in StunComponents serverStateAtServerTick)
        {
            if (clientStateAtServerTick.Base.IsStunned != serverStateAtServerTick.Base.IsStunned)
            {
                events.EnqueueServer((LocalPlayer.Events, new StunChangeEvent(serverStateAtServerTick.Base.IsStunned)));
            }
        }

        private void RaiseWeaponEvents(SimulationEvents events, EntityId actorId, in WeaponComponents clientState, in WeaponComponents serverState)
        {
            if (clientState.Base.State != serverState.Base.State)
            {
                events.EnqueueServer((LocalPlayer.Events, new WeaponStateChangedEvent(actorId, clientState.Base.State, serverState.Base.State)));
            }

            if (clientState.Consumable.AmmoInClip != serverState.Consumable.AmmoInClip)
            {
                events.EnqueueServer((LocalPlayer.Events, new AmmoChangeEvent(serverState.Consumable.AmmoInClip)));
            }
        }

        private void RespawnLocalPlayer(LocalRef<PlayerState> playerRef)
        {
            // TODO: ResetWeapon should be moved into system which runs before events are raised
            // TODO: move this check into property (right now it looks like a hack)
            TrooperInfo trooperInfo = playerInfo.GetTrooperInfo(playerRef.Value.TrooperClass);
            LocalPlayer = new ClientPlayer(playerRef, trooperInfo);
            var rse = new LocalPlayerRespawnedEvent(LocalPlayer);
            localPlayerRespawned.OnNext(rse);
        }

        private void SpawnLocalPlayer(LocalRef<PlayerState> playerRef)
        {
            TrooperInfo trooperInfo = playerInfo.GetTrooperInfo(playerRef.Value.TrooperClass);
            LocalPlayer = new ClientPlayer(playerRef, trooperInfo);
            localPlayerSpawned.OnNext(new LocalPlayerSpawnedEvent(LocalPlayer));
        }

        private void SpawnRemotePlayer(SimulationEvents events, PlayerRef playerRef, Tick at)
        {
            // redundant creation, state reapplied on the next line.
            var newRemotePlayer = new RemotePlayer(playerRef);

            newRemotePlayer.ApplyState(playerRef);
            newRemotePlayer.Events.Move.OnNext(newRemotePlayer.Transform);
            currentPlayers.Add(newRemotePlayer.Id, newRemotePlayer);

            var pje = new RemotePlayerJoinedEvent(
                newRemotePlayer,
                playerRef.Value.TrooperClass,
                playerRef.Value.Weapon.Base.Name);
            remotePlayerJoined.OnNext(pje);

            RiseEffectsEventsForSpawnedRemotePlayer(events, newRemotePlayer, at);
        }

        private void RaiseLeaved(PooledDictionary<EntityId, RemotePlayer> oldWorld)
        {
            foreach (var old in oldWorld)
            {
                if (!currentPlayers.ContainsKey(old.Key))
                {
                    var ple = new RemotePlayerLeavedEvent(old.Value);
                    remotePlayerLeaved.OnNext(ple);
                }
            }
        }

        private void RaiseHealing(
            SimulationEvents events,
            PlayerEventsBase playerEvents,
            ref PlayerState newPlayerRef)
        {
            if (newPlayerRef.Heals.Count > 0)
            {
                events.EnqueueServer((playerEvents, new HealingServerEvent(newPlayerRef.Heals)));
            }
        }

        private void SetTeamsAndZones()
        {
            Team1.ApplyState(new Team1Ref(serverStateAtServerTick));
            Team2.ApplyState(new Team2Ref(serverStateAtServerTick));

            var zoneStates = serverStateAtServerTick.Value.Zones;
            for (byte i = 0; i < zoneStates.Length; i++)
            {
                ref var zoneState = ref zoneStates[i];
                zones[zoneState.Id].ApplyState(new ZoneRef(serverStateAtServerTick, i));
            }
        }

        private void RaiseKilled(
            SimulationEvents events,
            PlayerEventsBase playerEvents,
            ref PlayerState clientPlayer,
            ref PlayerState serverPlayer,
            Tick serverTick)
        {
            if (clientPlayer.ServerEvents.LastKiller != serverPlayer.ServerEvents.LastKiller && serverPlayer.ServerEvents.LastKiller != 0)
            {
                var deathType = DeathType.Default;
                var damages = serverPlayer.Damages.Span;
                for (var i = 0; i < damages.Length; i++)
                {
                    ref var damage = ref damages[i];
                    // TODO: v.shimkovich smells but ok for softlaunch
                    if (damage.DamageSource.EntityType == EntityType.RegenerationOnKillSkill)
                    {
                        deathType = DeathType.RegenerationOnKill;
                        serverPlayer.Damages.RemoveAt(i);
                    }
                }

                var killedEvent = new KilledServerEvent(serverPlayer.Id, serverPlayer.ServerEvents.LastKiller, deathType, serverTick, serverPlayer.TrooperClass);
                events.EnqueueServer((playerEvents, killedEvent));
            }
        }

        private void RaiseSkillStateEvents(
            SimulationEvents events,
            PlayerEventsBase playerEvents,
            EntityId actorId,
            in SkillComponents prevSkill,
            in SkillComponents newSkill,
            Tick at)
        {
            if (prevSkill.Base.State != newSkill.Base.State)
            {
                events.EnqueueClient((playerEvents, new SkillStateChangedEvent(actorId, newSkill.Base.Name, prevSkill.Base.State, newSkill.Base.State, at)));
            }

            if (prevSkill.Aiming.Aiming != newSkill.Aiming.Aiming)
            {
                events.EnqueueClient((playerEvents, new SkillAimChangeEvent(newSkill.Base.Name, newSkill.Aiming.Aiming)));
            }

            if (prevSkill.Casting.Casting != newSkill.Casting.Casting)
            {
                events.EnqueueClient((playerEvents, new SkillCastChangedEvent(newSkill.Base.Name, newSkill.Casting.Casting)));
            }
        }

        private void RiseEffectsEventsForSpawnedRemotePlayer(SimulationEvents events, RemotePlayer newRemotePlayer, Tick at)
        {
            if (newRemotePlayer.Effects.Stun.IsStunned)
            {
                events.EnqueueServer((newRemotePlayer.Events,
                    new StunChangeEvent(newRemotePlayer.Effects.Stun.IsStunned)));
            }

            if (newRemotePlayer.Effects.Root.IsRooted)
            {
                events.EnqueueServer((newRemotePlayer.Events,
                    new RootChangeEvent(newRemotePlayer.Effects.Root.IsRooted)));
            }

            if (newRemotePlayer.Effects.Invisible.IsInvisible)
            {
                events.EnqueueServer((newRemotePlayer.Events,
                    new InvisibilityChangeEvent(newRemotePlayer.Effects.Invisible.IsInvisible)));
            }

            if (newRemotePlayer.Effects.Immortal.IsImmortal)
            {
                events.EnqueueServer((newRemotePlayer.Events,
                    new ImmortalityChangeEvent(newRemotePlayer.Id, newRemotePlayer.Effects.Immortal.IsImmortal, serverStateAtServerTick.Tick)));
            }

            if (newRemotePlayer.Effects.Lucky.IsLucky)
            {
                events.EnqueueServer((newRemotePlayer.Events,
                    new LuckyChangeEvent(newRemotePlayer.Effects.Lucky.IsLucky)));
            }

            SkillComponents prev = default;

            RaiseSkillStateEvents(events, newRemotePlayer.Events, newRemotePlayer.Id, in prev, in newRemotePlayer.StateRef.Value.Skill1, at);
            RaiseSkillStateEvents(events, newRemotePlayer.Events, newRemotePlayer.Id, in prev, in newRemotePlayer.StateRef.Value.Skill2, at);
            RaiseSkillStateEvents(events, newRemotePlayer.Events, newRemotePlayer.Id, in prev, in newRemotePlayer.StateRef.Value.Skill3, at);
            RaiseSkillStateEvents(events, newRemotePlayer.Events, newRemotePlayer.Id, in prev, in newRemotePlayer.StateRef.Value.Skill4, at);
            RaiseSkillStateEvents(events, newRemotePlayer.Events, newRemotePlayer.Id, in prev, in newRemotePlayer.StateRef.Value.Skill5, at);
        }
    }
}