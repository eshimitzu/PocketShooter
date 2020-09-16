using Heyworks.PocketShooter.Configuration;
using Heyworks.PocketShooter.Core.SchedulerManager;
using Heyworks.PocketShooter.Networking;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime;
using Heyworks.PocketShooter.UI.Core;
using Heyworks.PocketShooter.UI.HUD.PainHUD;
using Heyworks.PocketShooter.Utils;
using UnityEngine;
using Zenject;

namespace Heyworks.PocketShooter.UI.HUD
{
    /// <summary>
    /// Represents a screen with player HUD in battle.
    /// </summary>
    internal class BattleHUDScreen : BaseScreen
    {
        [SerializeField]
        private BattleHUDView mainBattleView;
        [SerializeField]
        private PainHUDView painView;
        [SerializeField]
        private DominationModeView dominationModeView;
        [SerializeField]
        private GameObject sniperView;
        [SerializeField]
        private MatchTimerView timerView;
        [SerializeField]
        private WeaponsConfig weaponsConfiguration;

        [Inject]
        private GameVisualSettings gameVisualSettings;

        private BattleHUDPresenter hudPresenter;
        private PainHUDPresenter painPresenter;
        private DominationViewPresenter dominationViewPresenter;
        private SniperViewPresenter sniperViewPresenter;

        /// <summary>
        /// Setup BattleHUDScreen.
        /// </summary>
        /// <param name="localCharacter">Local character.</param>
        /// <param name="container">Container.</param>
        /// <param name="room">The game room.</param>
        public void Setup(LocalCharacter localCharacter, ICharacterContainer container, Room room)
        {
            DisposePresenters();

            var simulation = room.LocalPlayerSimulation;

            hudPresenter = new BattleHUDPresenter(
                mainBattleView,
                localCharacter,
                simulation.Game,
                room,
                weaponsConfiguration,
                gameVisualSettings);

            sniperViewPresenter = new SniperViewPresenter(sniperView, localCharacter, mainBattleView);

            painPresenter = new PainHUDPresenter(painView, simulation.Game.LocalPlayer, container);
            dominationViewPresenter = new DominationViewPresenter(dominationModeView, timerView, simulation, room);
            var dominationPointersPresenter = new DominationPointersPresenter(simulation.Game, room.Team, room.Game.ModeInfo, dominationModeView);

            AddDisposablePresenter(hudPresenter);
            AddDisposablePresenter(sniperViewPresenter);
            AddDisposablePresenter(painPresenter);
            AddDisposablePresenter(dominationViewPresenter);
            AddDisposablePresenter(dominationPointersPresenter);
        }

        private void Update()
        {
            hudPresenter.Update();
        }
    }
}