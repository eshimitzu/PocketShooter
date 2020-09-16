using System;
using System.Collections.Generic;
using Heyworks.PocketShooter.Configuration;
using Heyworks.PocketShooter.Meta.Communication;
using Heyworks.PocketShooter.SocialConnections.Core;
using Heyworks.PocketShooter.SocialConnections.SocialNetworks;
using Heyworks.PocketShooter.Utils;
using Heyworks.PocketShooter.Utils.Extensions;
using Microsoft.Extensions.Logging;
using UniRx.Async;

namespace Heyworks.PocketShooter.Authorization
{
    public sealed class AuthorizationController
    {
        private readonly IWebApiClient webApiClient;
        private readonly IAppConfiguration appConfiguration;
        private readonly Dictionary<SocialNetworkName, ISocialNetwork> socialNetworks;

        public ISocialNetwork GameCenterSocialNetwork => socialNetworks[SocialNetworkName.GameCenter];

        public ISocialNetwork GooglePlaySocialNetwork => socialNetworks[SocialNetworkName.GooglePlay];

        public bool IsAuthorizationInProgress { get; private set; }

        public AuthorizationController(
            IWebApiClient webApiClient,
            IAppConfiguration appConfiguration)
        {
            this.webApiClient = webApiClient;
            this.appConfiguration = appConfiguration;

            socialNetworks = new Dictionary<SocialNetworkName, ISocialNetwork>(1);
//            socialNetworks.Add(SocialNetworkName.GameCenter, SocialNetworkManager.GameCenter);
            socialNetworks.Add(SocialNetworkName.GooglePlay, SocialNetworkManager.GooglePlay);
        }

        public async UniTask<ResponseOption<RegisterResponseData>> RegisterWithDevice()
        {
            IsAuthorizationInProgress = true;

            string deviceId = DeviceIdentity.GetDeviceId();

            AuthLog.Log.LogInformation(
                "Registering new user account on server {address} with device id {deviceId}...",
                webApiClient.Endpoint.Address,
                deviceId);

            var registerResponse = await webApiClient.RegisterDevice(
                new RegisterRequest
                {
                    ApplicationStore = DeviceIdentity.GetApplicationStoreName(),
                    Country = "BY",
                    BundleId = appConfiguration.BundleId,
                    ClientVersion = appConfiguration.Version,
                    DeviceId = deviceId,
                });

            if (registerResponse.IsOk)
            {
                AuthLog.Log.LogInformation("Device Registration operation has succeeded. Game config version {version}", registerResponse.Ok.Data.GameConfigVersion);
            }
            else
            {
                AuthLog.Log.LogError("Registration failed. Code {0}. Message: {1}.", registerResponse.Error.Code, registerResponse.Error.Message);
            }

            IsAuthorizationInProgress = false;

            return registerResponse;
        }

        public async UniTask<ResponseOption<LoginResponseData>> Login()
        {
            IsAuthorizationInProgress = true;

            ResponseOption<LoginResponseData> loginResponse = null;
            ISocialNetwork socialNetwork = GetAvailableForLoginSocialNetwork();

            while (loginResponse == null && socialNetwork != null)
            {
                var socialLoginResponse = await LoginWithSocialNetwork(socialNetwork);

                if (socialLoginResponse.IsOk)
                {
                    loginResponse = ResponseOk.CreateOption<LoginResponseData>(socialLoginResponse.Ok.Data);
                }
                else
                {
                    AuthLog.Log.LogWarning("Login with social network is not succeeded. Logout and retry silently. Code {code}. Reason: {reason}", socialLoginResponse.Error.Code, socialLoginResponse.Error.Message);
                    socialNetwork.LogOut();
                }

                socialNetwork = GetAvailableForLoginSocialNetwork();
            }

            if (socialNetwork == null)
            {
                loginResponse = await LoginWithDevice();
            }

            if (loginResponse.IsOk)
            {
                AuthLog.Log.LogInformation("Login has succeeded.");
            }
            else
            {
                AuthLog.Log.LogWarning("Login is not succeeded. Code {code}. Reason: {msg}", loginResponse.Error.Code, loginResponse.Error.Message);
            }

            IsAuthorizationInProgress = false;

            return loginResponse;
        }

        private ISocialNetwork GetAvailableForLoginSocialNetwork()
        {
            var authorizationControllers = new List<ISocialNetwork>(socialNetworks.Count);
            foreach (var authController in socialNetworks)
            {
                if (authController.Value.IsConnected)
                {
                    authorizationControllers.Add(authController.Value);
                }
            }

            return authorizationControllers.RandomObject();
        }

        private async UniTask<ResponseOption<SocialLoginResponseData>> LoginWithSocialNetwork(ISocialNetwork socialNetwork)
        {
            AuthLog.Log.LogInformation("Logging in with social network {sn}.", socialNetwork.Name);

            var socialResponse = await socialNetwork.LogIn();

            ResponseOption<SocialLoginResponseData> loginResponse;

            if (socialResponse.IsOk)
            {
                var loginToken = SocialLoginTokenStorage.GetToken(socialResponse.Ok.Data.UserId);

                switch (socialResponse.Ok.Data)
                {
                    case GooglePlayAccessData googlePlayAccessData:
                        AuthLog.Log.LogInformation(
                            "Running Google login with our server login token: {token}",
                            loginToken);

                        var googleLoginRequest = new GoogleLoginRequest()
                        {
                            ClientAccessToken = googlePlayAccessData.AccessToken,
                            SocialId = googlePlayAccessData.UserId,
                            LoginToken = loginToken,
                            DeviceId = DeviceIdentity.GetDeviceId(),
                            ApplicationStore = DeviceIdentity.GetApplicationStoreName(),
                            BundleId = appConfiguration.BundleId,
                            ClientVersion = appConfiguration.Version,
                        };

                        loginResponse = await webApiClient.LoginGooglePlay(googleLoginRequest);
                        break;
                    case GameCenterAccessData gameCenterAccessData:
                        throw new NotImplementedException("Game Center login is not implemented");
                    default:
                        throw new NotImplementedException("Login does not implemented for current access data");
                }

                if (loginResponse.IsOk)
                {
                    SocialLoginResponseData data = loginResponse.Ok.Data;
                    SocialLoginTokenStorage.StoreToken(
                        data.SocialConnection.InternalId,
                        new SocialLoginToken(
                            data.SocialConnection.LoginToken,
                            data.SocialConnection.LoginTokenExpirationDate));
                }
            }
            else
            {
                loginResponse = new ResponseOption<SocialLoginResponseData>(socialResponse.Error);
            }

            return loginResponse;
        }

        private async UniTask<ResponseOption<LoginResponseData>> LoginWithDevice()
        {
            var deviceId = DeviceIdentity.GetDeviceId();

            AuthLog.Log.LogInformation("Logging in with device id = {id}.", deviceId);

            var loginResponse = await webApiClient.LoginDevice(
                new LoginRequest
                {
                    DeviceId = deviceId,
                    ApplicationStore = DeviceIdentity.GetApplicationStoreName(),
                    BundleId = appConfiguration.BundleId,
                    ClientVersion = appConfiguration.Version,
                });

            return loginResponse;
        }
    }
}
