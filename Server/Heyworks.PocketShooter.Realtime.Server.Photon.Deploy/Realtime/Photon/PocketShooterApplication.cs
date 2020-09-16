using System.IO;
using System.Threading;
using Heyworks.PocketShooter.Meta.Communication;
using Heyworks.PocketShooter.Realtime.Hosting;
using Heyworks.PocketShooter.Realtime.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Heyworks.PocketShooter.Realtime.Server
{
    /// <summary>
    /// The Pocket Shooter Photon application.
    /// </summary>
    public class PocketShooterApplication : PocketShooterApplicationBase
    {
        /// <inheritdoc />
        protected override void Setup()
        {
            Directory.SetCurrentDirectory(BinaryPath);
            var initialized = Volatile.Read(ref host);
            if (initialized == null)
            {
                var builder = RuntimeHostBuilder
                            .Create(nameof(PocketShooterApplication), BinaryPath)
                            .ConfigureServices(services =>
                            {
                                services.AddHostedService<MetaClusterClientService>();
                                services.AddHostedService<GameManagementRuntime>();
                            });

                initialized = builder.Build();
                if (Interlocked.CompareExchange(ref host, initialized, null) != null)
                {
                    initialized.Dispose();
                }
                else
                {
                    SetupStatic(host);
                    host.Start();
                }
            }
        }
    }
}
