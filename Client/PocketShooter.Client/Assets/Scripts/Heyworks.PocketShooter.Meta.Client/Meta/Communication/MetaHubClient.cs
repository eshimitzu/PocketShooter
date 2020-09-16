using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Polly;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public abstract class MetaHubClient : IMetaHubClient
    {
        private readonly string hubName;
        private readonly IConnectionListener connectionListener;

        public MetaHubClient(
            IPEndPoint serverAddress,
            string hubName,
            string clientVersion,
            IAccessTokenProvider accessTokenProvider,
            ILoggerFactory loggerFactory,
            IConnectionListener connectionListener)
        {
            this.hubName = hubName;
            this.connectionListener = connectionListener;

            Connection =
                new MetaHubConnectionBuilder(serverAddress, accessTokenProvider, clientVersion, loggerFactory)
                .Build(hubName);

            Logger = loggerFactory.CreateLogger(GetType());

            Connection.Closed += Connection_Closed;
        }

        public async ValueTask ConnectAsync()
        {
            try
            {
                await StartConnectionAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An error has occurred while connecting to {hubName} hub.", hubName);

                throw;
            }
        }

        public async ValueTask DisconnectAsync()
        {
            try
            {
                await Connection.StopAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An error has occurred while disconnecting from {hubName} hub.", hubName);

                throw;
            }
        }

        protected async ValueTask SendAsync(string methodName)
        {
            try
            {
                await Connection.SendAsync(methodName);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An error has occurred while calling {methodName} hub method.", methodName);

                throw;
            }
        }

        protected async ValueTask SendAsync(string methodName, object arg1)
        {
            try
            {
                await Connection.SendAsync(methodName, arg1);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An error has occurred while calling {methodName} hub method.", methodName);
                throw;
            }
        }

        protected async ValueTask SendAsync(string methodName, object arg1, object arg2)
        {
            try
            {
                await Connection.SendAsync(methodName, arg1, arg2);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An error has occurred while calling {methodName} hub method.", methodName);
                throw;
            }
        }

        protected async ValueTask<ResponseOption> InvokeAsync(string methodName, object arg1)
        {
            try
            {
                var ok = await Connection.InvokeAsync<ResponseOption>(methodName, arg1);

                return ok;
            }
            catch (HubException ex)
            {
                return ResponseError.CreateOption(ApiErrorCode.InternalServerError, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An error has occurred while calling {methodName} hub method.", methodName);

                throw;
            }
        }

        protected async ValueTask<ResponseOption<TOk>> InvokeAsync<TOk>(string methodName)
        {
            try
            {
                return await Connection.InvokeAsync<ResponseOption<TOk>>(methodName);
            }
            catch (HubException ex)
            {
                return ResponseError.CreateOption<TOk>(ApiErrorCode.InternalServerError, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An error has occurred while calling {methodName} hub method.", methodName);

                throw;
            }
        }

        protected async ValueTask<ResponseOption<TOk>> InvokeAsync<TOk>(string methodName, object arg1)
        {
            try
            {
                var ok = await Connection.InvokeAsync<ResponseOption<TOk>>(methodName, arg1);

                return ok;
            }
            catch (HubException ex)
            {
                return ResponseError.CreateOption<TOk>(ApiErrorCode.InternalServerError, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An error has occurred while calling {methodName} hub method.", methodName);

                throw;
            }
        }

        protected ILogger Logger { get; }

        protected HubConnection Connection { get; }

        private Task StartConnectionAsync()
        {
            return Policy
                .Handle<Exception>()
                .WaitAndRetryForeverAsync(
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, nextAfter) =>
                    {
                        connectionListener.Disconnected(typeof(MetaHubClient));

                        if (exception != null)
                        {
                            Logger.LogWarning(
                                exception,
                                "An error has occurred while starting connection to {hubName}",
                                hubName);
                        }

                        Logger.LogWarning("Schedule next hub connect attempt after {nextAfter}", nextAfter);
                    })
                .ExecuteAsync(
                    async () =>
                    {
                        Logger.LogWarning("Starting connection to {hubName}", hubName);

                        await Connection.StartAsync();

                        Logger.LogWarning("Connected to {hubName}", hubName);
                        connectionListener.Connected(typeof(MetaHubClient));
                    });
        }

        private Task Connection_Closed(Exception exc)
        {
            connectionListener.Disconnected(typeof(MetaHubClient));

            if (exc != null)
            {
                Logger.LogWarning(exc, "Connection to {hubName} was closed with error", hubName);

                return StartConnectionAsync();
            }
            else
            {
                Logger.LogInformation("Connection to {hubName} was gracefully closed", hubName);

                return Task.CompletedTask;
            }
        }
    }
}