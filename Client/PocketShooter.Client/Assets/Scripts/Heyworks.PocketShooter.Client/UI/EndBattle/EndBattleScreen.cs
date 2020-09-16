using System.Collections.Generic;
using Heyworks.PocketShooter.Camera;
using Heyworks.PocketShooter.Character;
using Heyworks.PocketShooter.Core;
using Heyworks.PocketShooter.Core.SchedulerManager;
using Heyworks.PocketShooter.Meta.Communication;
using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.Modules.Analytics;
using Heyworks.PocketShooter.Realtime;
using Heyworks.PocketShooter.UI.Core;
using Microsoft.Extensions.Logging;
using UniRx.Async;
using UnityEngine;
using Zenject;

namespace Heyworks.PocketShooter.UI.EndBattle
{
    public class EndBattleScreen : BaseScreen
    {
        private const int DelayBeforeNextBattleMs = 3000;

        [SerializeField]
        private WinLoosView winLoosView;

        [SerializeField]
        private LobbyProfileBar profileBar;

        [SerializeField]
        private ResultTrooperCell[] trooperCells;

        [Inject]
        private ScreensController screensController;

        [Inject]
        private TrooperCreator trooperCreator;

        [Inject]
        private IGameHubClient gameHubClient;

        [Inject]
        private Main main;

        [Inject]
        private OrbitCamera orbitCamera;

        [Inject]
        private MapSceneManager mapSceneManager;

        public async void Setup(Room room, bool isDraw, bool weWin)
        {
            DisposePresenters();

            var presenter = new EndBattleScreenPresenter(winLoosView, isDraw, weWin);
            AddDisposablePresenter(presenter);

            var lobbyPresenter = new LobbyScreenPresenter(profileBar, main);
            AddDisposablePresenter(lobbyPresenter);

            var fightButtonPresenter = new LobbyFightButtonPresenter(profileBar, main);
            AddDisposablePresenter(fightButtonPresenter);
            profileBar.OnBackClick += () =>
            {
                mapSceneManager.UnloadCurrentMapScene();
                var screen = screensController.ShowScreen<LobbyScreen>();
                screen.Setup(main, true);
            };

            foreach (ResultTrooperCell resultTrooperCell in trooperCells)
            {
                resultTrooperCell.gameObject.SetActive(false);
            }

            profileBar.BackButton.gameObject.SetActive(false);
            profileBar.FightButton.gameObject.SetActive(false);

            await GetResults(room);

            profileBar.FightButton.gameObject.SetActive(true);
            profileBar.BackButton.gameObject.SetActive(true);
        }

        private void OnEnable()
        {
            orbitCamera?.EnableBlur(true);
        }

        private void OnDisable()
        {
            orbitCamera?.EnableBlur(false);
        }

        private async UniTask EnableButtonsAfterDelay(int delayMs)
        {
            await UniTask.Delay(delayMs);

            profileBar.FightButton.gameObject.SetActive(true);
            profileBar.BackButton.gameObject.SetActive(true);
        }

        private void FillResults(out MatchResultsData results)
        {
            results = new MatchResultsData();
            results.Stats = GenerateStats(123);
            results.IsWinner = Random.value < 0.5f;
            results.Reward = new PlayerReward(100, 10, 10);
            results.OtherPlayerStats = new List<PlayerMatchStatsData>();
            int otherPlayers = Random.Range(2, 4);

            for (int i = 0; i < otherPlayers; i++)
            {
                results.OtherPlayerStats.Add(GenerateStats(i));
            }
        }

        private PlayerMatchStatsData GenerateStats(int index)
        {
            var player = new PlayerMatchStatsData();
            player.Nickname = $"Player{index}";
            player.Kills = Random.Range(0, 20);
            player.Deaths = Random.Range(0, 20);
            player.TrooperClass = TrooperClass.Rambo;
            player.IsMVP = Random.value < 0.5f;

            return player;
        }

        private void ShowResults(MatchResultsData results)
        {
            List<PlayerMatchStatsData> players = new List<PlayerMatchStatsData>();
            players.AddRange(results.OtherPlayerStats);
            players.Sort((a, b) => a.Kills.CompareTo(b.Kills));
            players.Insert(0, results.Stats);

            for (var index = 0; index < players.Count; index++)
            {
                PlayerMatchStatsData player = players[index];
                ResultTrooperCell resultCell = trooperCells[index];
                resultCell.gameObject.SetActive(true);
                resultCell.Setup(trooperCreator, player, index);
            }

            trooperCells[0].Setup(results.Reward);
        }

        private async UniTask GetResults(Room room)
        {
            var matchResultsData = await gameHubClient.GetMatchResultsAsync(room.Id);

            if (matchResultsData.IsOk)
            {
                MatchResultsData matchResults = matchResultsData.Ok.Data;
                if (matchResults != null)
                {
                    AnalyticsManager.Instance.SendPlayerBattleFinish(matchResults);

                    main.MetaGame.ApplyPlayerReward(matchResults.Reward);
                    ShowResults(matchResults);
                    NetLog.Information($"Sync match results succeeded. {matchResults}");
                }
                else
                {
                    NetLog.Information($"Sync match results succeeded. results is null");
                }
            }
            else
            {
                NetLog.Log.LogWarning(
                    $"Get match results error. Reason: {matchResultsData.Error.Message} : {matchResultsData.Error.Code}");
            }
        }
    }
}