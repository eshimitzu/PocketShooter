using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Heyworks.PocketShooter.Meta.Communication
{
    internal class RetryMessageHandler : DelegatingHandler
    {
        private readonly TimeSpan requestTimeout = TimeSpan.FromSeconds(20);
        private readonly ILogger logger;
        private readonly IConnectionListener disconnectListener;

        public RetryMessageHandler(
            HttpClientHandler innerHandler,
            ILogger logger,
            IConnectionListener disconnectListener)
            : base(innerHandler)
        {
            this.logger = logger;
            this.disconnectListener = disconnectListener;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Policy
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>()
                .OrResult<HttpResponseMessage>(x => x.StatusCode != HttpStatusCode.Conflict && x.StatusCode != HttpStatusCode.Unauthorized && !x.IsSuccessStatusCode)
                .WaitAndRetryForeverAsync(retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (result, nextAfter) =>
                {
                    disconnectListener.Disconnected(typeof(HttpClient));

                    if (result.Result != null)
                    {
                        logger.LogWarning(
                            "Request to {requestUri} failed. Response status {statusCode}",
                            result.Result.RequestMessage.RequestUri,
                            result.Result.StatusCode);
                    }

                    if (result.Exception != null)
                    {
                        logger.LogWarning(result.Exception, "Request error details");
                    }

                    logger.LogWarning("Schedule next request after {nextAfter}", nextAfter);
                })
                .ExecuteAsync(async () =>
                {
                    using (var cts = GetCancellationTokenSource(cancellationToken))
                    {
                        HttpResponseMessage response = await base.SendAsync(request, cts.Token);

                        disconnectListener.Connected(typeof(HttpClient));

                        return response;
                    }
                });
        }

        private CancellationTokenSource GetCancellationTokenSource(CancellationToken cancellationToken)
        {
            var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(requestTimeout);

            return cts;
        }
    }
}
