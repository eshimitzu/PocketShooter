using System;
using Heyworks.PocketShooter.Core.SchedulerManager;
using Heyworks.PocketShooter.Realtime;
using Heyworks.PocketShooter.UI;
using Heyworks.PocketShooter.UI.Localization;
using Heyworks.PocketShooter.Utils;

public class BattleNotificationPresenter : IDisposable
{
    private BattleNotificationView battleNotificationView;

    private int killSeries;
    private SchedulerTask killschedulerTask;

    private Room room;
    private GameVisualSettings gameVisualSettings;

    private int notificationTimeToFinishMatch = 5;
    private int gameDuration = 5;

    public BattleNotificationPresenter(BattleNotificationView battleNotificationView, Room room, GameVisualSettings gameVisualSettings)
    {
        this.battleNotificationView = battleNotificationView;
        this.room = room;
        this.gameVisualSettings = gameVisualSettings;

        this.battleNotificationView.Setup(gameVisualSettings.NotificationFadeSpeed);

        notificationTimeToFinishMatch = gameVisualSettings.NotificationTimeToFinishMatch;

        gameDuration = room.Game.ModeInfo.GameDuration;
    }

    public void Update()
    {
        if (Constants.ToSeconds(gameDuration - room.GameTime) <= notificationTimeToFinishMatch &&
            notificationTimeToFinishMatch > -1)
        {
            if (notificationTimeToFinishMatch == 0)
            {
                battleNotificationView.ShowMessage(LocKeys.MatchEnded.Localized());
            }
            else
            {
                battleNotificationView.ShowMessage(notificationTimeToFinishMatch.ToString());
            }

            notificationTimeToFinishMatch--;
        }
    }

    public void Dispose()
    {
    }

    public void ProcessKill() 
    {
        killSeries++;

        if (killSeries > 1)
        {
            battleNotificationView.ShowMessage((LocKeys.KillSeries + Math.Min(killSeries, gameVisualSettings.MaxKillSeries)).Localized());
        }

        if (killschedulerTask != null)
        {
            SchedulerManager.Instance.RemoveSchedulerTask(killschedulerTask);
            killschedulerTask = null;
        }

        killschedulerTask = new SchedulerTask(SchedulerManager.Instance, () => { killSeries = 0; }, gameVisualSettings.KillSeriesTimeOut);
        SchedulerManager.Instance.AddSchedulerTask(killschedulerTask);
    }

    public void Killed()
    {
        killSeries = 0;

        if (killschedulerTask != null)
        {
            SchedulerManager.Instance.RemoveSchedulerTask(killschedulerTask);
            killschedulerTask = null;
        }
    }
}
