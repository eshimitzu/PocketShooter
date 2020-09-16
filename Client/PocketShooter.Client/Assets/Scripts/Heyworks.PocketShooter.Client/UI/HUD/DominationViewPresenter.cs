using System;
using System.Collections.Generic;
using Heyworks.PocketShooter.Configuration;
using Heyworks.PocketShooter.Core.SchedulerManager;
using Heyworks.PocketShooter.Realtime;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Simulation;
using Heyworks.PocketShooter.UI.Core;
using Heyworks.PocketShooter.Utils;
using UniRx;
using Zenject;

namespace Heyworks.PocketShooter.UI.HUD
{
    /// <summary>
    /// Represents DominationViewPresenter.
    /// </summary>
    internal class DominationViewPresenter : IDisposablePresenter
    {
        private readonly DominationModeView dominationModeView;
        private readonly MatchTimerView timerView;
        private readonly IClientSimulation simulation;
        private readonly Room room;

        private readonly List<IDisposable> subscriptions = new List<IDisposable>();

        [Inject]
        private ScreensController screensController;

        /// <summary>
        /// Initializes a new instance of the <see cref="DominationViewPresenter"/> class.
        /// </summary>
        /// <param name="dominationModeView">Domination mode view.</param>
        /// <param name="timerView">Timer view.</param>
        /// <param name="simulation">Simulation.</param>
        /// <param name="room">The game room.</param>
        public DominationViewPresenter(
            DominationModeView dominationModeView,
            MatchTimerView timerView,
            IClientSimulation simulation,
            Room room)
        {
            AssertUtils.NotNull(dominationModeView, nameof(dominationModeView));
            AssertUtils.NotNull(timerView, nameof(timerView));
            AssertUtils.NotNull(simulation, nameof(simulation));
            AssertUtils.NotNull(room, nameof(room));

            this.timerView = timerView;
            this.dominationModeView = dominationModeView;
            this.simulation = simulation;
            this.room = room;

            var config = room.Game.ModeInfo;
            dominationModeView.Setup(simulation.Game, room.Team, config);
            timerView.StartTimer(room.Game.Ticker.Current, config.GameDuration - room.GameTime);

            room.GameEnded.Subscribe(Game_Ended).AddTo(subscriptions);
            room.Game.StateUpdated.Subscribe(OnStateUpdated).AddTo(subscriptions);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            foreach (var subscription in subscriptions)
            {
                subscription.Dispose();
            }

            subscriptions.Clear();
        }

        private void Game_Ended(Game game)
        {
            timerView.StopTimer();
        }

        private void OnStateUpdated(int currentTime)
        {
            timerView.UpdateTimer(room.Game.Ticker.Current);
        }
    }
}