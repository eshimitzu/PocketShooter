using Microsoft.Extensions.Logging;
using Polly;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Heyworks.PocketShooter.Meta.Communication
{
    internal class AuthenticationMessageHandler : DelegatingHandler
    {
        private readonly IAccessTokenProvider accessTokenProvider;
        private readonly ILogger logger;

        private string accessToken;

        public AuthenticationMessageHandler(HttpMessageHandler innerHandler, IAccessTokenProvider accessTokenProvider, ILogger logger)
            : base(innerHandler)
        {
            this.accessTokenProvider = accessTokenProvider;
            this.logger = logger;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Policy
                .HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.Unauthorized)
                .RetryAsync(1, async (result, attemp) =>
                {
                    logger.LogInformation(
                            "Request to {requestUri} unauthorized. Request new access token.",
                            result.Result.RequestMessage.RequestUri);

                    accessToken = await accessTokenProvider.GetAccessToken();

                    logger.LogDebug("New access token received: {accessToken}", accessToken);
                })
                .ExecuteAsync(() =>
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    return base.SendAsync(request, cancellationToken);
                });
        }
    }
}
