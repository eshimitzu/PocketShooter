using System.Collections;
using System.Collections.Generic;
using Heyworks.PocketShooter.LocalSettings;
using Heyworks.PocketShooter.Meta.Communication;
using Heyworks.PocketShooter.SocialConnections.Core;
using Heyworks.PocketShooter.SocialConnections.SocialNetworks;
using Heyworks.PocketShooter.UI.Core;
using Heyworks.PocketShooter.UI.Localization;
using Heyworks.PocketShooter.UI.Settings;
using I2.Loc;
using Microsoft.Extensions.Logging;
using UniRx.Async;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Zenject;

namespace Heyworks.PocketShooter.UI.Popups
{
    public class SettingsPopup : BaseScreen
    {
        private readonly string[] languages =
        {
            LocKeys.Languages.English,
            LocKeys.Languages.Russian,
        };

        [Inject]
        private SocialConnectionController socialConnectionController;

        [Inject]
        private IGameHubClient gameHubClient;

        [Inject]
        private IAuthorizedWebApiClient authorizedWebApiClient;

        [SerializeField]
        private LobbyColorsPreset colorsPreset;

        [SerializeField]
        private Button closeButton;

        [SerializeField]
        private Button spaceCloseButton;

        [SerializeField]
        private CheckPointButton soundButton;

        [SerializeField]
        private Button googleSocialButton;

        [SerializeField]
        private Text googleSocialButtonText;

        [SerializeField]
        private Text googleSocialAccontName;

        [SerializeField]
        private Image googleSocialAccountImage;

        [SerializeField]
        private RectTransform languagesRoot;

        [SerializeField]
        private LanguageButton languageButtonPrefab;

        private List<LanguageButton> buttons = new List<LanguageButton>();

        private void OnEnable()
        {
            closeButton.onClick.AddListener(CloseButtonOnClick);
            spaceCloseButton.onClick.AddListener(CloseButtonOnClick);
            googleSocialButton.onClick.AddListener(GoogleAccButtonOnClick);
            soundButton.OnClick += SoundButtonOnClick;
        }

        private void OnDisable()
        {
            closeButton.onClick.RemoveListener(CloseButtonOnClick);
            spaceCloseButton.onClick.RemoveListener(CloseButtonOnClick);
            googleSocialButton.onClick.RemoveListener(GoogleAccButtonOnClick);
            soundButton.OnClick -= SoundButtonOnClick;
        }

        private void Start()
        {
            soundButton.IsCheckPointActive = GameSettings.SoundOn;

            foreach (string language in languages)
            {
                LanguageButton button = Instantiate(languageButtonPrefab, languagesRoot, false);
                button.Language = language;
                button.OnClick += ActivateLanguage;
                buttons.Add(button);
            }

            ActivateLanguage(GameSettings.CurrentLanguage);

            // TODO: v.shimkovich setup social view depending on SN
            if (socialConnectionController.SocialNetworkName != SocialNetworkName.GooglePlay)
            {
                googleSocialButton.gameObject.SetActive(false);
            }
            UpdateSocialButton();
        }

        private void ActivateLanguage(string globalLanguage)
        {
            foreach (LanguageButton languageButton in buttons)
            {
                languageButton.SetActive(languageButton.GlobalLanguage == globalLanguage);
            }

            GameSettings.CurrentLanguage = globalLanguage;
        }

        private void SoundButtonOnClick()
        {
            GameSettings.SoundOn = !GameSettings.SoundOn;
            soundButton.IsCheckPointActive = GameSettings.SoundOn;
        }

        private void CloseButtonOnClick()
        {
            Hide();
        }

        private async void GoogleAccButtonOnClick()
        {
            if (socialConnectionController.IsConnected)
            {
                socialConnectionController.Disconnect();
                await Reconnect();
            }
            else
            {
                ProgressHUD.Instance.Show();
                var connectResponse = await socialConnectionController.Connect();
                ProgressHUD.Instance.Hide();

                if (!connectResponse.IsOk)
                {
                    switch (connectResponse.Error.Code)
                    {
                        case ApiErrorCode.SocialNetworkAccessTokenReceiveFailed:
                        case ApiErrorCode.InvalidSocialConnectRequest:
                        case ApiErrorCode.InvalidSocialCredentials:
                            ScreensController.Instance.ShowMessageBox(
                                LocKeys.Social.Title.Localized(),
                                LocKeys.Social.GetSocialNetworkError(socialConnectionController.SocialNetworkName).Localized(),
                                new PopupOptionInfo(LocKeys.Social.RogerButton.Localized(), colorsPreset.GreenButton));
                            socialConnectionController.Disconnect();
                            break;
                        case ApiErrorCode.UserAlreadyConnected:
                            ScreensController.Instance.ShowMessageBox(
                                LocKeys.Social.Title.Localized(),
                                LocKeys.Social.UserAlreadyConnected.Localized(),
                                new PopupOptionInfo(LocKeys.Social.RogerButton.Localized(), colorsPreset.GreenButton));
                            socialConnectionController.Disconnect();
                            break;
                        case ApiErrorCode.SocialAccountConnectedToAnotherUser:
                            if (connectResponse.Error is SocialConnectResponseError error)
                            {
                                ScreensController.Instance.ShowMessageBox(
                                    LocKeys.Social.Title.Localized(),
                                    LocKeys.Social.GetSocialAccountSwitchOrSkipLocalized(
                                        error.DeviceUserNickname,
                                        socialConnectionController.SocialNetworkUser.FirstName,
                                        error.SocialUserNickname),
                                    new PopupOptionInfo(
                                        error.DeviceUserNickname,
                                        colorsPreset.GreenButton,
                                        () => socialConnectionController.Disconnect()),
                                    new PopupOptionInfo(
                                        error.SocialUserNickname,
                                        colorsPreset.BlueButton,
                                        async () =>
                                        {
                                            GooglePlayGamesStorage.SetServiceConnectedState();
                                            await Reconnect();
                                        }));
                            }
                            else
                            {
                                AuthLog.Log.LogError("Response error is of wrong type, expected {type}, actual {act}", typeof(SocialConnectResponseError), connectResponse.Error.GetType());
                                goto default;
                            }

                            break;
                        default:
                            ScreensController.Instance.ShowMessageBox(
                                LocKeys.Social.Title.Localized(),
                                LocKeys.Social.UnknownAuthError.Localized(),
                                new PopupOptionInfo(LocKeys.Social.RogerButton.Localized(), colorsPreset.GreenButton));
                            socialConnectionController.Disconnect();
                            break;
                    }
                }
            }

            UpdateSocialButton();
        }

        // TODO: v.shimkovich implement socialConnectionView?
        private void UpdateSocialButton()
        {
            googleSocialButtonText.SetLocalizedText(socialConnectionController.IsConnected ? LocKeys.LogOut : LocKeys.LogIn);

            var localize = googleSocialAccontName.GetComponent<Localize>();
            if (localize)
            {
                localize.enabled = !socialConnectionController.IsConnected;
            }

            if (socialConnectionController.IsConnected)
            {
                googleSocialAccontName.text = socialConnectionController.SocialNetworkUser.FirstName;

                StartCoroutine(SetAvatar());
            }
            else
            {
                switch (socialConnectionController.SocialNetworkName)
                {
                    case SocialNetworkName.GooglePlay:
                        googleSocialAccontName.SetLocalizedText(LocKeys.SocialNetworkGoogle);
                        break;
                    default:
                        googleSocialAccontName.text = default;
                        break;
                }

                googleSocialAccountImage.gameObject.SetActive(false);
            }
        }

        private IEnumerator SetAvatar()
        {
            Debug.Log(socialConnectionController.SocialNetworkUser.AvatarURL);
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(socialConnectionController.SocialNetworkUser.AvatarURL))
            {
                yield return uwr.SendWebRequest();

                if (uwr.isNetworkError || uwr.isHttpError)
                {
                    googleSocialAccountImage.gameObject.SetActive(false);
                }
                else
                {
                    Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                    googleSocialAccountImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    googleSocialAccountImage.gameObject.SetActive(true);
                }
            }
        }

        private async UniTask Reconnect()
        {
            ProgressHUD.Instance.Show(0f);

            await gameHubClient.DisconnectAsync();
            await gameHubClient.ConnectAsync();
            authorizedWebApiClient.StartNewSession();

            ProgressHUD.Instance.Hide();
        }
    }
}