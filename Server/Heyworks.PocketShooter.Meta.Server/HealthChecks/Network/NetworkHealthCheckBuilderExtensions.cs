using System;
using System.Collections.Generic;
using HealthChecks.Network;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class NetworkHealthCheckBuilderExtensions
    {
        const string UDP_NAME = "udp";

        public static IHealthChecksBuilder AddUdpHealthCheck(this IHealthChecksBuilder builder, Action<UdpHealthCheckOptions> setup, string name = default, HealthStatus? failureStatus = default, IEnumerable<string> tags = default)
        {
            var options = new UdpHealthCheckOptions();
            setup?.Invoke(options);

            return builder.Add(new HealthCheckRegistration(
               name ?? UDP_NAME,
               sp => new UdpHealthCheck(options),
               failureStatus,
               tags));
        }        
    }
}
