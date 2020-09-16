using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Heyworks.PocketShooter.Meta.Communication;
using Microsoft.Extensions.Logging;
using UniRx.Async;

#if !UNITY_ANDROID
using System;
#endif
namespace Heyworks.PocketShooter.SocialConnections.SocialNetworks
{
    /// <summary>
    /// Represents an object providing access to the Google Play social network.
    /// </summary>
    public sealed class GooglePlaySocialNetwork : ISocialNetwork
    {
#if UNITY_ANDROID
        private GooglePlayAccessData accessToken;
        private UniTaskCompletionSource initialized;
        private UniTaskCompletionSource<ResponseOption<SocialNetworkAccessData>> loggedIn;
#endif

        private static bool IsGooglePlayAvailableForAndroid
        {
            get
            {
                // TODO: v.shimkovich additional google play restrictions, implementation can be found in PT or troopers
                /*var configProvider = new GameConfigProvider();
                var deviceCountry = DeviceLocationWrapper.GetDeviceLocaleCountryCode();
                var operationSystemInfo = SystemInfo.operatingSystem;

                var isGooglePlayAvailableForAndroid = configProvider.IsSocialNetworkAvailable(
                    SocialNetworkName.GooglePlay,
                    deviceCountry,
                    operationSystemInfo);
                return isGooglePlayAvailableForAndroid;*/
                return true;
            }
        }

        /// <inheritdoc cref="ISocialNetwork"/>>
        public SocialNetworkName Name => SocialNetworkName.GooglePlay;

        /// <inheritdoc cref="ISocialNetwork"/>>
        public SocialNetworkUser Player { get; private set; }

        /// <inheritdoc cref="ISocialNetwork"/>>
        public bool IsConnected
        {
            get
            {
#if UNITY_ANDROID
                return IsGooglePlayAvailableForAndroid
                       && PlayGamesPlatform.Instance.IsAuthenticated()
                       && accessToken != null
                       && Player != null
                       && GooglePlayGamesStorage.IsServiceConnected();
#else
                return false;
#endif
            }
        }

        /// <inheritdoc cref="ISocialNetwork"/>>
        public async UniTask Initialize()
        {
#if UNITY_ANDROID
            if (!IsGooglePlayAvailableForAndroid)
            {
                return;
            }

            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
                .RequestIdToken()
                .Build();
            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Activate();

            if (GooglePlayGamesStorage.IsServiceConnected())
            {
                initialized = new UniTaskCompletionSource();
                PlayGamesPlatform.Instance.Authenticate(SessionSilentWokeUp, true);
                await initialized.Task;
            }
#endif
        }

        /// <inheritdoc cref="ISocialNetwork"/>>
        public async UniTask<ResponseOption<SocialNetworkAccessData>> LogIn()
        {
#if UNITY_ANDROID
            if (IsConnected)
            {
                return ResponseOk.CreateOption<SocialNetworkAccessData>(accessToken);
            }

            loggedIn = new UniTaskCompletionSource<ResponseOption<SocialNetworkAccessData>>();
            PlayGamesPlatform.Instance.Authenticate(AuthorizationCompleted, false);
            var response = await loggedIn.Task;
            return response;
#else
            throw new NotImplementedException("Google Play Games is not available.");
#endif
        }

        /// <inheritdoc cref="ISocialNetwork"/>>
        public void LogOut()
        {
#if UNITY_ANDROID
            PlayGamesPlatform.Instance.SignOut();
            GooglePlayGamesStorage.ResetServiceConnectedState();
            Player = null;
            accessToken = null;
#endif
        }

#if UNITY_ANDROID
        private void SessionSilentWokeUp(bool isAuthenticated)
        {
            AuthLog.Log.LogInformation("GooglePlaySocialNetwork.SessionSilentWokeUp(). isAuthenticated: {isAuth}", isAuthenticated);

            if (isAuthenticated)
            {
                InitializeAndTryGetToken();
            }

            initialized.TrySetResult();
        }

        private void AuthorizationCompleted(bool isAuthenticated)
        {
            if (isAuthenticated)
            {
                bool ok = InitializeAndTryGetToken();

                loggedIn.TrySetResult(
                    ok
                    ? ResponseOk.CreateOption<SocialNetworkAccessData>(accessToken)
                    : ResponseError.CreateOption<SocialNetworkAccessData>(ApiErrorCode.SocialNetworkAccessTokenReceiveFailed, "Failed to get PlayGames token"));
            }
            else
            {
                loggedIn.TrySetResult(
                    ResponseError.CreateOption<SocialNetworkAccessData>(ApiErrorCode.SocialNetworkAccessTokenReceiveFailed, "PlayGamesPlatform.Authenticate() is failed"));
            }
        }

        private bool InitializeAndTryGetToken()
        {
            var localUser = (PlayGamesLocalUser)PlayGamesPlatform.Instance.localUser;
            string userId = localUser.id;
            Player = new SocialNetworkUser(userId, localUser.userName, localUser.AvatarURL);

            string token = PlayGamesPlatform.Instance.GetIdToken();
            if (string.IsNullOrEmpty(token))
            {
                if (!GooglePlayGamesStorage.IsTokenExist(userId))
                {
                    return false;
                }

                token = GooglePlayGamesStorage.LoadToken(userId);
            }
            else
            {
                GooglePlayGamesStorage.SaveToken(userId, token);
            }

            accessToken = new GooglePlayAccessData(token, userId);

            return true;
        }
#endif
    }
}