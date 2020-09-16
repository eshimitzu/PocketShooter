using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace HealthChecks.Network
{
    public class UdpHealthCheck
        : IHealthCheck
    {
        private readonly UdpHealthCheckOptions _options;
        public UdpHealthCheck(UdpHealthCheckOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                foreach (var (host, port, request, timeout) in _options.ConfiguredHosts)
                {
                    using (var udpClient = new UdpClient())
                    {
                        udpClient.Connect(host, port);
                        udpClient.AllowNatTraversal(true);                       
        
                        // cannot check UDP port open until response, but response is sent only if right protocol
                        // still will get error if car is dead-unrechable
                        var receive = udpClient.ReceiveAsync();
                        var sent = await udpClient.SendAsync(request, request.Length);
                                                
                        if (!receive.Wait(timeout) || sent < 0)
                        {
                            return new HealthCheckResult(context.Registration.FailureStatus, description: $"Send and receive to host {host}:{port} failed");
                        }
                    }
                }

                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
            }
        }
    }
}
