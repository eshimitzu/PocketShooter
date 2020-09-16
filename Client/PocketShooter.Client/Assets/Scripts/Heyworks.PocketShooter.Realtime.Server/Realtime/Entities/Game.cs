using System;
using System.Collections.Generic;
using Collections.Pooled;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities.Skills;
using Microsoft.Extensions.Logging;

namespace Heyworks.PocketShooter.Realtime.Entities
{
    public class Game : IServerGame
    {
        private readonly DominationModeInfo modeInfo;
        private readonly PooledDictionary<EntityId, ServerPlayer> currentPlayers;
        private readonly PooledDictionary<byte, Zone> zones;
        private readonly ILogger logger;

        public Game(GameRef gameStateRef, DominationModeInfo modeInfo, ILogger logger)
        {
            this.GameStateRef = gameStateRef;
            this.modeInfo = modeInfo;
            this.currentPlayers = new PooledDictionary<EntityId, ServerPlayer>(modeInfo.MaxPlayers);
            this.logger = logger;

            Team1 = new Team(new Team1Ref(gameStateRef), modeInfo.Map.Teams[0]);
            Team2 = new Team(new Team2Ref(gameStateRef), modeInfo.Map.Teams[1]);

            var zonesInfo = modeInfo.Map.Zones;
            this.zones = new PooledDictionary<byte, Zone>(zonesInfo.Length);
            for (byte zoneIndex = 0; zoneIndex < zonesInfo.Length; zoneIndex++)
            {
                var zoneInfo = zonesInfo[zoneIndex];
                gameStateRef.Value.Zones[zoneIndex] = new ZoneState(zoneInfo.Id, TeamNo.None, 0, false);
                zones.Add(zoneInfo.Id, new Zone(new ZoneRef(gameStateRef, zoneIndex), zoneInfo));
            }

            MatchResult = new DominationMatchResult();
            ConsumablesState = new ConsumablesMatchState();
        }

        public GameRef GameStateRef { get; private set; }

        public IReadOnlyDictionary<EntityId, ServerPlayer> Players => currentPlayers;

        public ServerPlayer GetServerPlayer(EntityId id)
        {
            currentPlayers.TryGetValue(id, out var player);
            return player;
        }

        public IReadOnlyDictionary<byte, Zone> Zones => zones;

        public Team Team1 { get; }

        public Team Team2 { get; }

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

        /// <summary>
        /// Gets or sets a value indicating whether game is ended.
        /// </summary>
        public bool IsEnded { get; set; }

        public long EndTime { get; set; }

        public DominationMatchResult MatchResult { get; }

        public ConsumablesMatchState ConsumablesState { get; }

        IPlayer IGame.GetPlayer(EntityId id) => GetServerPlayer(id);

        public GameState GetState()
        {
            return GameStateRef.Value.Clone();
        }

        public void ApplyState(GameRef newGameStateRef)
        {
            GameStateRef = newGameStateRef;
            var players = GameStateRef.Value.Players.Span;
            for (byte i = 0; i < players.Length; i++)
            {
                ref var playerState = ref players[i];
                if (currentPlayers.TryGetValue(playerState.Id, out var player))
                {
                    player.ApplyState(new PlayerRef(GameStateRef, i));
                }
            }

            Team1.ApplyState(new Team1Ref(GameStateRef));
            Team2.ApplyState(new Team2Ref(GameStateRef));

            var zoneStates = GameStateRef.Value.Zones;
            for (byte i = 0; i < zoneStates.Length; i++)
            {
                ref var zoneState = ref zoneStates[i];
                zones[zoneState.Id].ApplyState(new ZoneRef(GameStateRef, i));
            }
        }

        public void RemoveTrooper(EntityId trooperId)
        {
            currentPlayers.Remove(trooperId);
            var playerStates = GameStateRef.Value.Players;
            // TODO: write some class which is both index and array
            for (int i = 0; i < playerStates.Count; i++)
            {
                ref var playerState = ref playerStates.Span[i];
                if (playerState.Id == trooperId)
                {
                    playerStates.RemoveAt(i);

                    // update players indexes.
                    foreach (var currentPlayer in currentPlayers)
                    {
                        var index = (byte)playerStates.FindIndex(s => s.Id == currentPlayer.Key);
                        currentPlayer.Value.ApplyState(new PlayerRef(GameStateRef, index));
                    }

                    break;
                }
            }
        }

        public void RespawnTrooper(string nickname, EntityId trooperId, TeamNo teamNo, TrooperInfo trooperInfo)
        {
            var player = GetServerPlayer(trooperId);
            if (player != null)
            {
                RemoveTrooper(trooperId);
            }

            AddTrooper(nickname, trooperId, teamNo, trooperInfo);
        }

        private ServerPlayer AddTrooper(
            string nickname,
            EntityId newTrooperId,
            TeamNo teamNo,
            TrooperInfo trooperInfo)
        {
            var team = GetTeam(teamNo);

            var spawnPoint = modeInfo.Map.Teams[(byte)teamNo - 1].SpawnPoints[team.NextSpawnPoint];
            var transform = FpsTransformComponent.CreateTransform(spawnPoint);

            var playerState = SafeCreatePlayerState(nickname, newTrooperId, teamNo, trooperInfo, transform);

            var playerStates = GameStateRef.Value.Players;
            playerStates.Add(playerState);
            var newPlayer = new ServerPlayer(new PlayerRef(GameStateRef, (byte)(playerStates.Count - 1)), trooperInfo, ConsumablesState.PlayerStats[newTrooperId]);
            currentPlayers.Add(newTrooperId, newPlayer);

            logger.LogInformation("Trooper {playerState} was added at {serverTick} server tick", playerState, GameStateRef.Tick);

            return newPlayer;
        }

        private PlayerState SafeCreatePlayerState(string nickname, EntityId newTrooperId, TeamNo teamNo, TrooperInfo trooperInfo, in FpsTransformComponent transform)
        {
            try
            {
                return PlayerState.Create(
                    newTrooperId,
                    nickname,
                    teamNo,
                    trooperInfo,
                    transform);
            }
            catch (InvalidOperationException ex) when (ex.Message == "Sequence contains no matching element")
            {
                throw new ClientException(ClientErrorCode.NotAReleaseFeature, newTrooperId, "Fix before release. Client should pass CharacterMetaId, not CharacterMetaData", ex);
            }
        }
    }
}