using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Serialization;
using Microsoft.Extensions.Logging;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public class WebApiClient : IWebApiClient
    {
        private readonly HttpClient httpClient;

        private readonly IDataSerializer dataSerializer;
        private readonly ILogger<WebApiClient> logger;

        public WebApiClient(IPEndPoint endpoint, string clientVersion, IDataSerializer dataSerializer, ILoggerFactory loggerFactory, IConnectionListener disconnectListener)
        {
            Endpoint = endpoint;
            this.logger = loggerFactory.CreateLogger<WebApiClient>();
            this.dataSerializer = dataSerializer;

            this.httpClient = new HttpClient(new RetryMessageHandler(new HttpClientHandler(), logger, disconnectListener));
            httpClient.BaseAddress = new Uri($"{Uri.UriSchemeHttp}://{endpoint}/api/");
            httpClient.DefaultRequestHeaders.Add(RequestHeaders.ClientVersion, clientVersion);
        }

        public IPEndPoint Endpoint { get; private set; }

        public async ValueTask<ResponseOption<RegisterResponseData>> RegisterDevice(RegisterRequest registerRequest)
        {
            try
            {
                var responseMessage = await httpClient.PostAsync(
                    "account/register/device",
                    dataSerializer.CreateRequestContent(registerRequest));

                return await responseMessage.GetResponseOptionAsync<RegisterResponseData>(dataSerializer);
            }
            catch (Exception ex)
            {
                LogActionError(nameof(IWebApiClient.RegisterDevice), ex);
                throw;
            }
        }

        public async ValueTask<ResponseOption<LoginResponseData>> LoginDevice(LoginRequest loginRequest)
        {
            try
            {
                var responseMessage = await httpClient.PostAsync(
                    "account/login/device",
                    dataSerializer.CreateRequestContent(loginRequest));

                return await responseMessage.GetResponseOptionAsync<LoginResponseData>(dataSerializer);
            }
            catch (Exception ex)
            {
                LogActionError(nameof(IWebApiClient.LoginDevice), ex);
                throw;
            }
        }

        public async ValueTask<ResponseOption<SocialLoginResponseData>> LoginGooglePlay(GoogleLoginRequest loginRequest)
        {
            try
            {
                var responseMessage = await httpClient.PostAsync(
                    "account/login/google",
                    dataSerializer.CreateRequestContent(loginRequest));

                return await responseMessage.GetResponseOptionAsync<SocialLoginResponseData>(dataSerializer);
            }
            catch (Exception ex)
            {
                LogActionError(nameof(IWebApiClient.LoginGooglePlay), ex);
                throw;
            }
        }

        public ValueTask<ResponseOption<SocialLoginResponseData>> LoginGameCenter()
        {
            throw new NotImplementedException();
        }

        private void LogActionError(string actionName, Exception ex)
        {
            logger.LogError(ex, "An error has occurred while executing {actionName} action.", actionName);
        }
    }
}
