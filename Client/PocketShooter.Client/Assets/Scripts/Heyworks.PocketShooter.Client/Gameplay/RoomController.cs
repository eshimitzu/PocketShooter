using System;
using System.Collections.Generic;
using System.Linq;
using Heyworks.PocketShooter.Core;
using Heyworks.PocketShooter.Core.SchedulerManager;
using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.Modules.Analytics;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Simulation;
using Heyworks.PocketShooter.UI.Core;
using Heyworks.PocketShooter.UI.EndBattle;
using Heyworks.PocketShooter.UI.HUD;
using Heyworks.PocketShooter.UI.TrooperSelection;
using Microsoft.Extensions.Logging;
using UniRx;
using UniRx.Async;

namespace Heyworks.PocketShooter.Gameplay
{
    internal class RoomController
    {
        private readonly ZonesManager zonesManager;
        private readonly ScreensController screensController;
        private readonly Room room;
        private readonly CharacterManager characterManager;
        private readonly BotManager botManager;
        private readonly MapSceneManager mapSceneManager;
        private readonly MetaGame metaGame;
        private readonly List<IDisposable> subscriptions = new List<IDisposable>();

        private bool isGameStarted;

        internal bool IsGameEnded = false;

        internal Room CurrentRoom => room;

        internal Game Game => room.Game;

        internal IClientSimulation LocalPlayerSimulation => room.LocalPlayerSimulation;

        internal INetworkSimulation NetworkSimulationController => room.NetworkSimulation;

        internal RoomController(
            Room room,
            ZonesManager zonesManager,
            CharacterManager characterManager,
            BotManager botManager,
            ScreensController screensController,
            MapSceneManager mapSceneManager,
            MetaGame metaGame)
        {
            this.room = room;
            this.zonesManager = zonesManager;
            this.characterManager = characterManager;
            this.botManager = botManager;
            this.screensController = screensController;
            this.mapSceneManager = mapSceneManager;
            this.metaGame = metaGame;

            room.GameStarted.Subscribe(Room_OnGameStarted).AddTo(subscriptions);
            room.GameEnded.Subscribe(Room_OnGameEnded).AddTo(subscriptions);
            room.BotSpawned.Subscribe(Room_OnBotSpawned).AddTo(subscriptions);
            room.BotRespawned.Subscribe(Room_OnBotRespawned).AddTo(subscriptions);

            mapSceneManager.MapSceneLoaded += Map_Loaded;

            AnalyticsManager.Instance.SetupBattleManagers(characterManager, botManager);
        }

        internal void Dispose()
        {
            characterManager.Dispose();
            botManager.Dispose();
            zonesManager.Dispose();

            subscriptions.ForEach(_ => _.Dispose());
            subscriptions.Clear();

            room.Dispose();

            AnalyticsManager.Instance.SendMatchPerfomance(mapSceneManager.CurrentMapSceneInfo.MapName);

            mapSceneManager.MapSceneLoaded -= Map_Loaded;

            metaGame.Army.RemoveSelectedOffensive(Game.ConsumablesState.PlayerStats[Game.LocalPlayerId].UsedOffensives);
            metaGame.Army.RemoveSelectedSupport(Game.ConsumablesState.PlayerStats[Game.LocalPlayerId].UsedSupports);
        }

        private void StartMatchActions()
        {
            ScreensController.Instance.HideCurrentScreen();
            var screen = ScreensController.Instance.ShowScreen<TrooperSelectionScreen>();
            screen.Setup(new TrooperSelectionSpawnHandler(LocalPlayerSimulation), LocalPlayerSimulation, room, null);

            zonesManager.SpawnZones(room);

            AnalyticsManager.Instance.LogStartMatch();
        }

        private void Map_Loaded()
        {
            if (isGameStarted)
            {
                StartMatchActions();
            }
        }

        private void Room_OnGameStarted(Game game)
        {
            GameLog.Trace("Game started.");

            // TODO: a.dezhurko introduce game controller here to handle game events and to separate room logic from game logic. Refactor when join and start events will be separated
            game.RemotePlayerJoined.Subscribe(Game_RemotePlayerJoined).AddTo(subscriptions);
            game.RemotePlayerLeaved.Select(x => x.RemotePlayer).Subscribe(Game_RemotePlayerLeaved).AddTo(subscriptions);
            game.WorldCleared.Subscribe(Game_WorldCleared).AddTo(subscriptions);
            game.RemotePlayerRespawned.Subscribe(RemotePlayer_OnRespawned).AddTo(subscriptions);
            game.LocalPlayerSpawned.Subscribe(Game_LocalPlayerSpawned).AddTo(subscriptions);
            game.LocalPlayerRespawned.Subscribe(ClientPlayer_OnRespawned).AddTo(subscriptions);

            isGameStarted = true;
            if (mapSceneManager.IsMapLoaded)
            {
                StartMatchActions();
            }
        }

        private void Room_OnGameEnded(Game game)
        {
            UnityEngine.Debug.Log($"Room_OnGameEnded");
            GameLog.Information("Game ended.");

            ref readonly var team1 = ref game.Team1.State;
            ref readonly var team2 = ref game.Team2.State;
            bool isDraw = false;
            bool weWin = false;

            if (team1.Score == team2.Score)
            {
                isDraw = true;
            }
            else
            {
                var winner = team1.Score > team2.Score ? team1 : team2;
                weWin = room.Team == winner.Number;
            }

            IsGameEnded = true;

            var screen = screensController.ShowScreen<EndBattleScreen>();
            screen.Setup(room, isDraw, weWin);

            AnalyticsManager.Instance.SendMatchPerfomance(mapSceneManager.CurrentMapSceneInfo.MapName);

            AnalyticsManager.Instance.SavePlayerBattleFinishData(AnalyticsManager.FinishBattleReason.Normal, isDraw, weWin);
        }

        private void Game_LocalPlayerSpawned(LocalPlayerSpawnedEvent spawnedEvent)
        {
            GameLog.Information("Local player spawned.");

            var clientPlayer = spawnedEvent.ClientPlayer;
            clientPlayer.Events.Killed.Subscribe(ClientPlayer_OnKilled).AddTo(subscriptions);

            var character = characterManager.CreateLocalCharacter(
            clientPlayer,
            room.LocalPlayerSimulation,
            room.NetworkSimulation);

            var battleHudScreen = screensController.ShowScreen<BattleHUDScreen>();
            battleHudScreen.Setup(character, characterManager, room);
        }

        private async void ClientPlayer_OnKilled(KilledServerEvent kse)
        {
            GameLog.Information("Local player with id {playerId} is dead.", kse.Killed);

            ScreensController.Instance.HideCurrentScreen();

            await UniTask.Delay(3000);

            if (IsGameEnded)
            {
                return;
            }

            var screen = ScreensController.Instance.ShowScreen<TrooperSelectionScreen>();
            screen.Setup(new TrooperSelectionRespawnHandler(LocalPlayerSimulation), LocalPlayerSimulation, room, kse.LastClass);
        }

        private void ClientPlayer_OnRespawned(LocalPlayerRespawnedEvent respawnedEvent)
        {
            GameLog.Information("Local player with id {ServerEvent} respawned.", respawnedEvent);

            characterManager.DeleteCharacter(respawnedEvent.ClientPlayer.Id);

            var clientPlayer = respawnedEvent.ClientPlayer;
            clientPlayer.Events.Killed.Subscribe(ClientPlayer_OnKilled).AddTo(subscriptions);

            var localCharacter = characterManager.CreateLocalCharacter(
            respawnedEvent.ClientPlayer,
            room.LocalPlayerSimulation,
            room.NetworkSimulation);

            screensController.HideCurrentScreen();
            var battleHudScreen = screensController.ShowScreen<BattleHUDScreen>();
            battleHudScreen.Setup(localCharacter, characterManager, room);
        }

        private void Game_RemotePlayerJoined(RemotePlayerJoinedEvent rpj)
        {
            IRemotePlayer remotePlayer = rpj.RemotePlayer;

            GameLog.Information("Player joined with id {playerId}.", remotePlayer.Id);

            if (remotePlayer.IsAlive)
            {
                characterManager.DeleteCharacter(remotePlayer);

                characterManager.CreateRemoteCharacter(
                    remotePlayer,
                    rpj.TrooperClass,
                    room.Team,
                    room.NetworkSimulation.TickProvider,
                    room.LocalPlayerSimulation);
            }
        }

        private void Game_RemotePlayerLeaved(IRemotePlayer remotePlayer)
        {
            GameLog.Information("Player leaved with id {playerId}.", remotePlayer.Id);

            characterManager.DeleteCharacter(remotePlayer);
        }

        private void Game_WorldCleared(ResetWorldEvent rwe)
        {
            characterManager.DeleteRemoteCharacters();
        }

        private void RemotePlayer_OnRespawned(RemotePlayerRespawnedEvent rse)
        {
            GameLog.Information("Remote player with id {playerId} respawned.", rse.RemotePlayer.Id);

            characterManager.DeleteCharacter(rse.RemotePlayer.Id);

            IRemotePlayer remotePlayer = Game.Players[rse.RemotePlayer.Id];

            characterManager.CreateRemoteCharacter(
                remotePlayer,
                rse.TrooperClass,
                room.Team,
                room.NetworkSimulation.TickProvider,
                room.LocalPlayerSimulation);
        }

        // TODO: a.dezhurko Move all about bots to bot manager.
        private void Room_OnBotSpawned(BotSimulationData botSpawnedData)
        {
            GameLog.Information("Control taken on bot with id {botId}", botSpawnedData.BotInfo.Id);

            botManager.CreateBotTrooper(
                botSpawnedData.BotInfo,
                botSpawnedData.Context.LocalPlayer,
                botSpawnedData.ClientSimulation,
                room.NetworkSimulation.TickProvider,
                room.NetworkSimulation,
                room.Team != botSpawnedData.Context.LocalPlayer.Team);
        }

        private void Room_OnBotRespawned(BotSimulationData data)
        {
            var botPlayer = data.Context.LocalPlayer;

            GameLog.Information("Bot trooper with id {playerId} respawned.", botPlayer.Id);

            botManager.DeleteBot(botPlayer.Id);
            botManager.CreateBotTrooper(
                data.BotInfo,
                botPlayer,
                data.ClientSimulation,
                room.NetworkSimulation.TickProvider,
                room.NetworkSimulation,
                room.Team != data.Context.LocalPlayer.Team);
        }
    }
}
