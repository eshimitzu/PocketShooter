using Heyworks.PocketShooter.Meta.Serialization;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public sealed class DeviceAccessTokenProvider : IAccessTokenProvider
    {
        private readonly HttpClient httpClient;

        private readonly LoginRequest loginRequest;
        private readonly IDataSerializer dataSerializer;
        private readonly ILogger logger;

        private string accessToken;

        public DeviceAccessTokenProvider(
            IPEndPoint endpoint,
            string clientVersion,
            LoginRequest loginRequest,
            IDataSerializer dataSerializer,
            ILoggerFactory loggerFactory,
            IConnectionListener connectionListener)
        {
            this.loginRequest = loginRequest;
            this.dataSerializer = dataSerializer;
            this.logger = loggerFactory.CreateLogger<DeviceAccessTokenProvider>();

            this.httpClient = new HttpClient(new RetryMessageHandler(new HttpClientHandler(), logger, connectionListener));
            httpClient.BaseAddress = new Uri($"{Uri.UriSchemeHttp}://{endpoint}/api/");
            httpClient.DefaultRequestHeaders.Add(RequestHeaders.ClientVersion, clientVersion);
        }

        public async Task<string> GetAccessToken()
        {
            if (accessToken == null)
            {
                try
                {
                    var responseMessage = await httpClient.PostAsync(
                        "account/login/device",
                        dataSerializer.CreateRequestContent(loginRequest));

                    var responseOption = await responseMessage.GetResponseOptionAsync<LoginResponseData>(dataSerializer);

                    accessToken = responseOption.IsOk ? responseOption.Ok.Data.AuthToken : null;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error has occurred during device access token request");

                    throw;
                }
            }

            return accessToken;
        }
    }
}
