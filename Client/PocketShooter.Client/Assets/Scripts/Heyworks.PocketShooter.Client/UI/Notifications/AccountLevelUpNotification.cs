using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.UI.Core;
using Heyworks.PocketShooter.UI.Popups;
using UnityEngine;

namespace Heyworks.PocketShooter.UI.Notifications
{
    public class AccountLevelUpNotification : Notification
    {
        private static readonly string LastShownLevelKey = "LastShownLevel";
        private readonly Player player;
        private readonly IPlayerConfigurationBase playerConfiguration;

        public AccountLevelUpNotification(Player player, IPlayerConfigurationBase playerConfiguration)
        {
            this.player = player;
            this.playerConfiguration = playerConfiguration;
        }

        private int LastShownLevel
        {
            get => PlayerPrefs.GetInt(LastShownLevelKey, player.Level);
            set => PlayerPrefs.SetInt(LastShownLevelKey, value);
        }

        public override void Notify()
        {
            CheckLevelRewards();
        }

        private void CheckLevelRewards()
        {
            if (player.Level - LastShownLevel > 0)
            {
                var reward = playerConfiguration.GetLevelUpReward(LastShownLevel, player.Level);
                var levelUpPopup = ScreensController.Instance.ShowPopup<AccountLevelUpPopup>();
                levelUpPopup.Setup(reward, LastShownLevel, player.Level);
            }

            LastShownLevel = player.Level;
        }
    }
}
