using System;
using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.UI.Core;
using Heyworks.PocketShooter.UI.Popups;
using Microsoft.Extensions.Logging;
using UniRx;
using UnityEngine;
using WebSocketSharp;

namespace Heyworks.PocketShooter.UI.Notifications
{
    public class ContentUpgradeNotification : Notification
    {
        private const string ItemProgressKey = "ARMY_ITEM_PROGRESS";
        private readonly Army army;

        public ContentUpgradeNotification(Army army)
        {
            this.army = army;

            army.ItemProgressStarted.Subscribe(OnItemStarted);
            army.ItemProgressCompleted.Subscribe(OnItemCompleted);
        }

        public override void Notify()
        {
            var itemData = LoadItemInProgressData();
            if (itemData == null)
            {
                return;
            }

            var item = army.FindItem(itemData.Id);
            if (item.Level > itemData.Level || item.Grade > itemData.Grade)
            {
                var content = army.FindContent(itemData.Id);
                SaveItemInProgressData(null);
                var screen = ScreensController.Instance.ShowPopup<UpgradeContentPopup>();
                screen.Setup(content, item.IsFirstLevelOnGrade);
            }
        }

        private static void SaveItemInProgressData(ItemInProgressData value)
        {
            if (value == null)
            {
                PlayerPrefs.SetString(ItemProgressKey, string.Empty);
            }
            else
            {
                var data = JsonUtility.ToJson(value);
                PlayerPrefs.SetString(ItemProgressKey, data);
            }
        }

        private static ItemInProgressData LoadItemInProgressData()
        {
            var data = PlayerPrefs.GetString(ItemProgressKey, string.Empty);
            if (data.IsNullOrEmpty())
            {
                return null;
            }

            try
            {
                return JsonUtility.FromJson<ItemInProgressData>(data);
            }
            catch (Exception e)
            {
                GameLog.Log.LogError("Can't deserialize item progress. " + e.Message);

                return null;
            }
        }

        private void OnItemStarted(ArmyItemProgress progress)
        {
            var item = army.FindItem(progress.ItemId);
            SaveItemInProgressData(
                new ItemInProgressData()
                {
                    Id = item.Id,
                    Level = item.Level,
                    Grade = item.Grade,
                });
        }

        private void OnItemCompleted(ArmyItemProgress progress)
        {
            if (ScreensController.Instance.CurrentScreen is LobbyScreen)
            {
                SaveItemInProgressData(null);
                var screen = ScreensController.Instance.ShowPopup<UpgradeContentPopup>();
                screen.Setup(progress.ToContentIdentity(), progress.IsFirstLevelOnGrade());
            }
        }

        [Serializable]
        private class ItemInProgressData
        {
            [SerializeField]
            public int Id;
            [SerializeField]
            public int Level;
            [SerializeField]
            public Grade Grade;
        }
    }
}