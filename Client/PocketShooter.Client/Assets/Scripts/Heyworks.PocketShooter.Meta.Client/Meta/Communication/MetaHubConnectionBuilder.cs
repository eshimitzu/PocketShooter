using System;
using System.Net;
using System.Reflection;
using Heyworks.PocketShooter.Meta.Serialization;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public class MetaHubConnectionBuilder
    {
        private readonly Uri baseUrl;
        private readonly IAccessTokenProvider accessTokenProvider;
        private readonly string clientVersion;
        private readonly ILoggerFactory loggerFactory;

        public MetaHubConnectionBuilder(
            IPEndPoint endpoint,
            IAccessTokenProvider accessTokenProvider,
            string clientVersion,
            ILoggerFactory loggerFactory)
        {
            this.baseUrl = new Uri($"{Uri.UriSchemeHttp}://{endpoint}");
            this.accessTokenProvider = accessTokenProvider;
            this.clientVersion = clientVersion;
            this.loggerFactory = loggerFactory;
        }

        public HubConnection Build(string hubName)
        {
            var connectionOptions = new HttpConnectionOptions
            {
                Url = new Uri(baseUrl, hubName),
                Transports = HttpTransportType.WebSockets,
                AccessTokenProvider = accessTokenProvider.GetAccessToken,
            };

            connectionOptions.Headers.Add(RequestHeaders.ClientVersion, clientVersion);

            var jsonProtocolOptions = new JsonHubProtocolOptions();
            UpdateWithDefaultSerializerSettings(jsonProtocolOptions);

            return new HubConnection(
                new HttpConnectionFactory(Options.Create(connectionOptions), loggerFactory),
                new JsonHubProtocol(Options.Create(jsonProtocolOptions)),
                loggerFactory);
        }

        private static void UpdateWithDefaultSerializerSettings(JsonHubProtocolOptions options)
        {
            // Workaround to set PayloadSerializerSettings for unsigned Json.Net.dll

            PropertyInfo prop = options.GetType().GetProperty(
                "PayloadSerializerSettings",
                BindingFlags.Public | BindingFlags.Instance);

            if (prop != null && prop.CanWrite)
            {
                prop.SetValue(options, new DefaultSerializerSettings(), null);
            }
            else
            {
                UnityEngine.Debug.LogError("Can't update JsonHubProtocolOptions with DefaultSerializerSettings.");

                // NOTE: a.dezhurko For some reason the exception is not shown in the console.
                throw new InvalidOperationException(
                    "Can't update JsonHubProtocolOptions with DefaultSerializerSettings.");
            }
        }
    }
}