using System.Collections;
using System.Linq;
using System.Text;
using Heyworks.PocketShooter;
using Heyworks.PocketShooter.Configuration;
using Heyworks.PocketShooter.Core;
using Heyworks.PocketShooter.Networking;
using Heyworks.PocketShooter.Realtime.Configuration;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Entities.Weapon;
using Heyworks.PocketShooter.Realtime.Service;
using Heyworks.PocketShooter.Realtime.Simulation;
using Heyworks.PocketShooter.Utils;
using UnityEngine;
using Zenject;

/// <summary>
/// This class is using Unity's coroutines to update the debugger window every WaitForSeconds() yield.
/// To add new stats to debugger window, append them to the debugInfoSb StringBuilder object in
/// CreateDebugInfoStringBuilder() method.
/// </summary>
public class Debugger : MonoBehaviour
{
    [SerializeField]
    private DebugUIView debugUIView;

    [SerializeField]
    private float redrawDebugInfoInterval = 0.5f;

    [Inject]
    private IAppConfiguration appConfiguration = null;

    [Inject]
    private IRealtimeConfiguration realtimeConfiguration = null;

    [Inject]
    private FpsToText fpsToText;

    private StringBuilder debugInfoSb;

    private bool isDebugUIActive;
    private string commitHash;
    private Main main;
    private RealtimeRunBehavior realtime;
    private int playerTeam;
    private int otherTeam;

    private Game Game => realtime._Game;

    private IRttProvider RttProvider => realtime._RoomController.CurrentRoom.Connection;

    private TimeManager TimeManager => realtime._TimeManager;

    private IBuffersStatsProvider BuffersStatsProvider => realtime._BuffersStatsProvider;

    public void SetDebuggerActive(bool isActive)
    {
        if (isActive)
        {
            isDebugUIActive = true;
            debugUIView.gameObject.SetActive(isDebugUIActive);
            fpsToText.gameObject.SetActive(isDebugUIActive);
            StartCoroutine(RedrawDebugInfoCoroutine());
        }
        else
        {
            isDebugUIActive = false;
            debugUIView.gameObject.SetActive(isDebugUIActive);
            fpsToText.gameObject.SetActive(isDebugUIActive);
            StopCoroutine(RedrawDebugInfoCoroutine());
        }
    }

    private void Start()
    {
        InitializeDebuggerInfo();
        fpsToText.gameObject.SetActive(isDebugUIActive);
        debugUIView.gameObject.SetActive(isDebugUIActive);
    }

    private void InitializeDebuggerInfo()
    {
        realtime = FindObjectOfType<RealtimeRunBehavior>();
        main = FindObjectOfType<Main>();
        commitHash = GetCommitHash();
        debugInfoSb = new StringBuilder(500);
    }

    private string GetCommitHash()
    {
        var file = Resources.Load<TextAsset>("commithash");
        if (file)
        {
            return file.text;
        }

        return string.Empty;
    }

    private void UpdateTeamSizes()
    {
        if (Game == null)
        {
            return;
        }

        var myTeam = Game.LocalPlayer?.Team ?? TeamNo.First;

        playerTeam = Game.Players.Count(p => p.Value.Team == myTeam);
        otherTeam = Game.Players.Count(p => p.Value.Team != myTeam);

        if (Game.LocalPlayer != null)
        {
            playerTeam++;
        }
    }

    private IEnumerator RedrawDebugInfoCoroutine()
    {
        while (isDebugUIActive)
        {
            UpdateTeamSizes();
            debugUIView.Setup(CreateDebugInfoStringBuilder().ToString());
            yield return new WaitForSeconds(redrawDebugInfoInterval);
        }
    }

    private StringBuilder CreateDebugInfoStringBuilder()
    {
        debugInfoSb.Clear();

        debugInfoSb.Append(appConfiguration.Version);
        debugInfoSb.AppendLine();

        if (!string.IsNullOrEmpty(commitHash))
        {
            debugInfoSb.Append(commitHash);
            debugInfoSb.AppendLine();
        }

        debugInfoSb.AppendFormat("meta: {0}", main.CurrentMetaServerAddress.ToString());
        debugInfoSb.AppendLine();

        debugInfoSb.AppendFormat("meta: {0}", main.DefaultMatchType.ToString());
        debugInfoSb.AppendLine();

        debugInfoSb.AppendFormat("meta: {0}", main.DefaultMatchMap.ToString());
        debugInfoSb.AppendLine();

        if (realtime == null)
        {
            return debugInfoSb;
        }

        if (realtime._Game == null)
        {
            return debugInfoSb;
        }

        debugInfoSb.AppendFormat("realtime: {0}", realtime._CurrentRealtimeServerAddress.Address.ToString());
        debugInfoSb.AppendLine();

        debugInfoSb.AppendFormat("{0} - {1}", playerTeam, otherTeam);
        debugInfoSb.AppendLine();

        if (realtime._TimeManager == null ||
                realtime._BuffersStatsProvider == null)
        {
            return debugInfoSb;
        }

        debugInfoSb.AppendFormat("rtt: {0}, last rtt: {1}, rtt var: {2}",
            RttProvider.RoundTripTimeMs, RttProvider.LastRoundTripTimeMs, RttProvider.RoundTripTimeVariance);
        debugInfoSb.AppendLine();

        debugInfoSb.AppendFormat("tick: {0}, world: {1}, delay: {2}",
            TimeManager.Tick, TimeManager.WorldTick, TimeManager.Tick - TimeManager.WorldTick);
        debugInfoSb.AppendLine();

        debugInfoSb.AppendFormat("adjustment client / world: {0} / {1}",
            TimeManager.ClientTickerAdjustment, TimeManager.WorldTickerAdjustment);
        debugInfoSb.AppendLine("\n");

        debugInfoSb.Append("server buffer");
        debugInfoSb.AppendLine();

        debugInfoSb.AppendFormat($"cur / target: {BuffersStatsProvider.ServerInputBufferSize} / {TimeManager.TargetInputBufferSizeOnServer:0.00}");
        debugInfoSb.AppendLine();

        debugInfoSb.AppendFormat($"mean / var: {TimeManager.InputBufferSizeOnServerMean:0.00} / {TimeManager.InputBufferSizeOnServerVariance:0.00}");
        debugInfoSb.AppendLine("\n");

        debugInfoSb.Append("client buffer");
        debugInfoSb.AppendLine();

        debugInfoSb.AppendFormat($"cur / target: {BuffersStatsProvider.WorldStatesBufferSize} / {TimeManager.TargetWorldStatesBufferSize:0.00}");
        debugInfoSb.AppendLine();

        debugInfoSb.AppendFormat($"mean / var: {TimeManager.WorldStatesBufferSizeMean:0.00} / {TimeManager.WorldStatesBufferSizeVariance:0.00}");
        debugInfoSb.AppendLine("\n");

        debugInfoSb.AppendFormat("inerpolator: {0}", realtimeConfiguration.EnableInterpolator);
        debugInfoSb.AppendLine();

        debugInfoSb.AppendFormat("inerpolator: {0}", realtimeConfiguration.InterpolateLostTicks);
        debugInfoSb.AppendLine();

        return debugInfoSb;
    }
}
