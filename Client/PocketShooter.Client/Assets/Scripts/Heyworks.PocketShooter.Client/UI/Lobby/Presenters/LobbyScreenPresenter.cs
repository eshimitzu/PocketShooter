using System;
using System.Collections.Generic;
using Heyworks.PocketShooter.Core;
using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.UI.Core;
using Heyworks.PocketShooter.UI.Popups;
using UniRx;
using XenStudio.UI;

namespace Heyworks.PocketShooter.UI
{
    public class LobbyScreenPresenter : IDisposablePresenter
    {
        private readonly LobbyProfileBar profileBar;
        private readonly Main main;
        private readonly Player player;
        private readonly List<IDisposable> subscriptions = new List<IDisposable>();

        public LobbyScreenPresenter(LobbyProfileBar profileBar, Main main)
        {
            this.profileBar = profileBar;
            this.main = main;
            player = main.MetaGame.Player;

            profileBar.ProfileView.ChangeNameButton.onClick.AddListener(ProfileBar_OnClick);

            main.MetaGame.GameStateChanged += Lobby_GameStateChanged;

            player.NicknameChanged.Subscribe(Player_NicknameChanged).AddTo(subscriptions);
            player.CashChanged.Subscribe(Player_CashChanged).AddTo(subscriptions);
            player.GoldChanged.Subscribe(Player_GoldChanged).AddTo(subscriptions);
            player.LevelChanged.Subscribe(Player_LevelChanged).AddTo(subscriptions);
            player.ExperienceInLevelChanged.Subscribe(Player_ExperienceInLevelChanged).AddTo(subscriptions);

            UpdateProfile();
        }

        public void Dispose()
        {
            profileBar.ProfileView.ChangeNameButton.onClick.RemoveListener(ProfileBar_OnClick);

            main.MetaGame.GameStateChanged -= Lobby_GameStateChanged;

            foreach (var subscription in subscriptions)
            {
                subscription.Dispose();
            }

            subscriptions.Clear();
        }

        private void Lobby_Error(string msg)
        {
            EasyMessageBox.Show(msg, layout: MessageLayout.Vertical);
        }

        private void Lobby_ConnectingTimeout(string msg)
        {
            EasyMessageBox.Show(msg, layout: MessageLayout.Vertical);
        }

        private void Lobby_JoiningRoomTimeout(string msg)
        {
            EasyMessageBox.Show(msg, layout: MessageLayout.Vertical);
        }

        private void UpdateProfile()
        {
            if (player != null)
            {
                profileBar.ProfileView.ProfileUsernameLabel.text = player.Nickname;
                profileBar.GoldLabel.text = player.Gold.ToString();
                profileBar.BucksLabel.text = player.Cash.ToString();

                UpdateLevel(player.Level);
                UpdateExperience(player.ExperienceInLevel);
            }
        }

        private void UpdateLevel(int level)
        {
            profileBar.ProfileView.ProfileLevelLabel.text = player.Level.ToString();
        }

        private void UpdateExperience(int exp)
        {
            if (!player.HasMaxLevel)
            {
                float experienceProgress = (float)exp / player.MaxExperienceInLevel;

                profileBar.ProfileView.ProfileLevelProgress.Progress = experienceProgress;
                profileBar.ProfileView.ProfileLevelProgressPercent.text = ((int)(experienceProgress * 100f)) + "%";
            }
            else
            {
                profileBar.ProfileView.ProfileLevelProgress.Progress = 1f;
                profileBar.ProfileView.ProfileLevelProgressPercent.text = "100%";
            }
        }

        private void ProfileBar_OnClick()
        {
            var popup = ScreensController.Instance.ShowPopup<ChangeNamePopup>();
            popup.Setup(main.MetaGame);
        }

        private void Lobby_GameStateChanged()
        {
            UpdateProfile();
        }

        private void Player_NicknameChanged(string nickname)
        {
            profileBar.ProfileView.ProfileUsernameLabel.text = nickname;
        }

        private void Player_CashChanged(int cash)
        {
            profileBar.BucksLabel.text = cash.ToString();
        }

        private void Player_GoldChanged(int gold)
        {
            profileBar.GoldLabel.text = gold.ToString();
        }

        private void Player_LevelChanged(int level)
        {
            UpdateLevel(level);
        }

        private void Player_ExperienceInLevelChanged(int exp)
        {
            UpdateExperience(exp);
        }
    }
}