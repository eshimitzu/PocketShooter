using System;
using Heyworks.PocketShooter.Meta.Communication;
using Heyworks.PocketShooter.SocialConnections.SocialNetworks;
using Microsoft.Extensions.Logging;
using UniRx.Async;

namespace Heyworks.PocketShooter.SocialConnections.Core
{
    public sealed class SocialConnectionController
    {
        private readonly IAuthorizedWebApiClient authorizedWebApiClient;
#if UNITY_IOS
        private readonly ISocialNetwork socialNetwork = SocialNetworkManager.GameCenter;
#elif UNITY_ANDROID
        private readonly ISocialNetwork socialNetwork = SocialNetworkManager.GooglePlay;
#else
        private readonly ISocialNetwork socialNetwork = SocialNetworkManager.GooglePlay;
#endif

        public SocialNetworkName SocialNetworkName => socialNetwork.Name;

        public SocialNetworkUser SocialNetworkUser => socialNetwork.Player;

        public SocialConnectionController(IAuthorizedWebApiClient authorizedWebApiClient)
        {
            this.authorizedWebApiClient = authorizedWebApiClient;
        }

        public bool IsConnected => socialNetwork.IsConnected;

        public async UniTask<ResponseOption<SocialConnectResponseData>> Connect()
        {
            var socialResponse = await socialNetwork.LogIn();

            ResponseOption<SocialConnectResponseData> connectResponse;

            if (socialResponse.IsOk)
            {
                AuthLog.Log.LogInformation("Running connect with {sn}", socialNetwork.Name);

                switch (socialResponse.Ok.Data)
                {
                    case GooglePlayAccessData googlePlayAccessData:
                        var googleConnectRequest = new GoogleConnectRequest()
                        {
                            ClientAccessToken = googlePlayAccessData.AccessToken,
                            SocialId = googlePlayAccessData.UserId,
                        };

                        connectResponse = await authorizedWebApiClient.ConnectGooglePlay(googleConnectRequest);
                        break;
                    case GameCenterAccessData gameCenterAccessData:
                        throw new NotImplementedException("Game Center connect is not implemented");
                    default:
                        throw new NotImplementedException("Connect does not implemented for current access data");
                }

                if (connectResponse.IsOk)
                {
                    AuthLog.Log.LogInformation($"Connect succeeded.");

                    SocialConnectResponseData data = connectResponse.Ok.Data;
                    SocialLoginTokenStorage.StoreToken(
                        data.SocialConnection.InternalId,
                        new SocialLoginToken(
                            data.SocialConnection.LoginToken,
                            data.SocialConnection.LoginTokenExpirationDate));

                    switch (socialResponse.Ok.Data)
                    {
                        case GooglePlayAccessData _:
                            GooglePlayGamesStorage.SetServiceConnectedState();
                            break;
                    }
                }
                else
                {
                    AuthLog.Log.LogWarning("Connect is not succeeded. Code: {code}. Reason: {msg}", connectResponse.Error.Code, connectResponse.Error.Message);
                }
            }
            else
            {
                AuthLog.Log.LogWarning("Login to social network is not succeeded. Code {code}. Reason: {reason}", socialResponse.Error.Code, socialResponse.Error.Message);
                connectResponse = new ResponseOption<SocialConnectResponseData>(socialResponse.Error);
            }

            return connectResponse;
        }

        public void Disconnect()
        {
            AuthLog.Log.LogInformation("Logout from social network.");

            socialNetwork.LogOut();
        }
    }
}
