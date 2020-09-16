using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Heyworks.PocketShooter.Camera;
using Heyworks.PocketShooter.Character;
using Heyworks.PocketShooter.Core;
using Heyworks.PocketShooter.Core.SchedulerManager;
using Heyworks.PocketShooter.Realtime;
using Heyworks.PocketShooter.Realtime.Simulation;
using Heyworks.PocketShooter.UI.Core;
using Heyworks.PocketShooter.UI.HUD;
using Heyworks.PocketShooter.UI.Localization;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Heyworks.PocketShooter.UI.TrooperSelection
{
    public class TrooperSelectionScreen : BaseScreen
    {
        [SerializeField]
        private BaseSelectionView selectionView;

        [SerializeField]
        private LobbyRosterView rosterView;

        [SerializeField]
        private Button fightButton;

        [SerializeField]
        private DominationModeView dominationModeView;

        [SerializeField]
        private MatchTimerView timerView;

        [SerializeField]
        private Button menuButton;

        [SerializeField]
        private IconsFactory itemsFactory;

        [Inject]
        private TrooperCreator trooperCreator;

        [Inject]
        private OrbitCamera orbitCamera;

        [Inject]
        private Main main;

        private Room room;

        private BaseSelectionPresenter selectionPresenter;

        private void Update()
        {
            selectionPresenter?.Update();
        }

        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed.")]
        public void Setup(ITrooperSelectionHandler handler, IClientSimulation simulation, Room room, TrooperClass? lastClass)
        {
            this.room = room;
            DisposePresenters();

            string itemName = null;
            if (lastClass.HasValue)
            {
                itemName = LocKeys.GetTooperNameKey(lastClass.Value);
            }

            var rosterPresenter = new LobbyRosterPresenter(
                rosterView,
                itemsFactory,
                RosterType.Trooper,
                room.PlayerInfo.Troopers.Select(trooper => new TrooperInfoRosterItem(trooper))
                    .OrderBy(trooper => trooper.Power)
                    .ToList<ILobbyRosterItem>(),
                new TrooperSelectionHandler(main.MetaGame),
                main.MetaGame);

            AddDisposablePresenter(rosterPresenter);

            var trooperSelectionPresenter = new TrooperSelectionPresenter(rosterPresenter, fightButton, handler);
            AddDisposablePresenter(trooperSelectionPresenter);

            selectionPresenter = new BaseSelectionPresenter(selectionView, trooperCreator, itemsFactory);

            AddDisposablePresenter(selectionPresenter);

            var dominationViewPresenter = new DominationViewPresenter(
                dominationModeView,
                timerView,
                simulation,
                room);
            AddDisposablePresenter(dominationViewPresenter);

            rosterPresenter.OnSelectedCardChanged += cardPresenter => selectionPresenter.ShowCard(cardPresenter, true);
        }

        private void OnEnable()
        {
            orbitCamera?.EnableBlur(true);
            menuButton.onClick.AddListener(OnMenuButtonClick);
        }

        private void OnDisable()
        {
            orbitCamera?.EnableBlur(false);
            menuButton.onClick.RemoveListener(OnMenuButtonClick);
        }

        private void OnMenuButtonClick() => room.ManualDisconnect();
    }
}