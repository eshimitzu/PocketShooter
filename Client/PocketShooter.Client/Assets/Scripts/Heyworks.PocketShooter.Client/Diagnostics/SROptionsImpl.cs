using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Heyworks.PocketShooter.Configuration;
using Heyworks.PocketShooter.Core;
using Heyworks.PocketShooter.Meta.Communication;
using Heyworks.PocketShooter.Networking;
using Heyworks.PocketShooter.Realtime.Configuration;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Service;
using Heyworks.PocketShooter.Realtime.Simulation;
using Heyworks.PocketShooter.Skills;
using Heyworks.PocketShooter.UI.Core;
using Heyworks.PocketShooter.UI.Debugging;
using Heyworks.PocketShooter.Utils;
using UnityEngine;
using Zenject;

namespace Heyworks.PocketShooter.Diagnostics
{
    /// <summary>
    /// Represents class for debug stuff.
    /// </summary>
    public class SROptionsImpl : MonoBehaviour, ISROptionsImpl
    {
        [SerializeField]
        private JumpSkillSpec jumpSkillSpec = null;

        [SerializeField]
        private Debugger debugger;

        [Inject]
        private IRealtimeConfiguration realtimeConfiguration = null;

        [Inject]
        private IAppConfiguration appConfiguration = null;

        [Inject]
        private IGameHubClient gameHubClient;

        private bool drawDebugUI;

        private RealtimeRunBehavior realtime;
        private Main main;

        private TimeManager TimeManager => realtime._TimeManager;

        private IRttProvider RttProvider => realtime._RoomController.CurrentRoom.Connection;

        private SRServerOptions serverOptions;

        private void Start()
        {
            realtime = FindObjectOfType<RealtimeRunBehavior>();
            main = FindObjectOfType<Main>();

            SROptions.Current.SetOptionsImpl(this);

            serverOptions = new SRServerOptions(realtime, main, appConfiguration);
            SRDebug.Instance.AddOptionContainer(serverOptions);
        }

        private void OnDestroy()
        {
            serverOptions.Dispose();
            SRDebug.Instance.RemoveOptionContainer(serverOptions);
        }

        #region [SROptions]

        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed.")]
        public float SkillJumpAngle
        {
            get => jumpSkillSpec.Angle;
            set => jumpSkillSpec.Angle = value;
        }

        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed.")]
        public float SkillJumpSpeed
        {
            get => jumpSkillSpec.Speed;
            set => jumpSkillSpec.Speed = value;
        }

        public void DecreaseMSAA()
        {
            QualitySettings.antiAliasing = 0;
        }

        public void IncreaseMSAA()
        {
            QualitySettings.antiAliasing = 2;
        }

        public void DecreaseQuality()
        {
            QualitySettings.DecreaseLevel(true);
        }

        public void IncreaseQuality()
        {
            QualitySettings.IncreaseLevel(true);
        }

        public void AddResources()
        {
            gameHubClient.Cheat_AddResources(1000, 1000);
        }

        public void UnlockContent()
        {
            gameHubClient.Cheat_UnlockContent();
        }

        public void AccountLevelUp()
        {
            gameHubClient.Cheat_LevelUpPlayer();
        }

        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed.")]
        public void EnableInterpolator()
        {
            ((RealtimeConfiguration)realtimeConfiguration).ToggleInterpolator();
        }

        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed.")]
        public void InterpolateLostTicks()
        {
            ((RealtimeConfiguration)realtimeConfiguration).ToggleInterpolateLostTicks();
        }

        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed.")]
        public void RandomizeWorldTick()
        {
            TimeManager.RandomizeWorldTick();
        }

        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed.")]
        public void RandomizeSimulationTick()
        {
            TimeManager.RandomizeSimulationTick();
        }

        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed.")]
        public void AddBot()
        {
            if (realtime._RoomController != null)
            {
                var room = realtime._RoomController.CurrentRoom;
                room.NetworkService.QueueCommand(default(RequestBotCommandData));
            }
        }

        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed.")]
        public void ShowCheats()
        {
            if (realtime._RoomController?.LocalPlayerSimulation is IClientSimulation simulation)
            {
                var popup = ScreensController.Instance.ShowPopup<CheatsPopup>();
                popup.Setup(realtime);
            }
        }

        public void NextMatchType()
        {
            main.DefaultMatchType = Next(main.DefaultMatchType);
        }

        public void NextMatchMap()
        {
            main.DefaultMatchMap = Next(main.DefaultMatchMap);
        }

        public void DebugUI()
        {
            drawDebugUI = !drawDebugUI;
            debugger.SetDebuggerActive(drawDebugUI);
        }

        private static MatchType Next(MatchType target)
        {
            var nextValueQuery = System.Enum.GetValues(typeof(MatchType)).Cast<MatchType>().SkipWhile(e => e != target).Skip(1);
            return nextValueQuery.Count() != 0
                ? nextValueQuery.First()
                : default;
        }

        private static MapNames Next(MapNames target)
        {
            var nextValueQuery = System.Enum.GetValues(typeof(MapNames)).Cast<MapNames>().SkipWhile(e => e != target).Skip(1);
            return nextValueQuery.Count() != 0
                ? nextValueQuery.First()
                : default;
        }

        #endregion
    }
}
