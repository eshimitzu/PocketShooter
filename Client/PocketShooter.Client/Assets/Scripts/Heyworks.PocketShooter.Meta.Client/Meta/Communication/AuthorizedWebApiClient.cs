using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Serialization;
using Microsoft.Extensions.Logging;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public class AuthorizedWebApiClient : IAuthorizedWebApiClient
    {
        private readonly IPEndPoint endpoint;
        private readonly string clientVersion;
        private readonly IAccessTokenProvider accessTokenProvider;
        private readonly IDataSerializer dataSerializer;
        private readonly ILogger<WebApiClient> logger;
        private readonly IConnectionListener connectionListener;

        private HttpClient httpClient;

        public AuthorizedWebApiClient(IPEndPoint endpoint, string clientVersion, IAccessTokenProvider accessTokenProvider, IDataSerializer dataSerializer, ILoggerFactory loggerFactory, IConnectionListener connectionListener)
        {
            this.endpoint = endpoint;
            this.clientVersion = clientVersion;
            this.accessTokenProvider = accessTokenProvider;
            this.dataSerializer = dataSerializer;
            this.logger = loggerFactory.CreateLogger<WebApiClient>();
            this.connectionListener = connectionListener;

            CreateHttpClient();
        }

        public void StartNewSession()
        {
            CreateHttpClient();
        }

        public async ValueTask<ResponseOption<SocialConnectResponseData>> ConnectGooglePlay(SocialConnectRequest connectRequest)
        {
            try
            {
                var responseMessage = await httpClient.PostAsync(
                    "account/connect/google",
                    dataSerializer.CreateRequestContent(connectRequest));

                return await responseMessage.GetResponseOptionAsync<SocialConnectResponseData, SocialConnectResponseError>(dataSerializer);
            }
            catch (Exception ex)
            {
                LogActionError(nameof(IAuthorizedWebApiClient.ConnectGooglePlay), ex);
                throw;
            }

        }

        private void CreateHttpClient()
        {
            this.httpClient = new HttpClient(new AuthenticationMessageHandler(
                new RetryMessageHandler(new HttpClientHandler(), logger, connectionListener),
                accessTokenProvider,
                logger));

            httpClient.BaseAddress = new Uri($"{Uri.UriSchemeHttp}://{endpoint}/api/");
            httpClient.DefaultRequestHeaders.Add(RequestHeaders.ClientVersion, clientVersion);
        }

        private void LogActionError(string actionName, Exception ex)
        {
            logger.LogError(ex, "An error has occurred while executing {actionName} action.", actionName);
        }
    }
}