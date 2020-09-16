using Heyworks.PocketShooter.Meta.Communication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Heyworks.PocketShooter.Meta.Server
{
    public class MetaServerFront
    {
        public IWebHost Create(string[] args, string basePath)
        {
            return new WebHostBuilder()
                .UseContentRoot(basePath)
                .UseKestrel(x => x.ListenAnyIP(5000))
                .ConfigureAppConfiguration((hostingContext, configBuilder) =>
                {
                    var env = hostingContext.HostingEnvironment;

                    configBuilder
                    .SetBasePath(env.ContentRootPath)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

                    configBuilder.AddEnvironmentVariables();

                    if (args != null)
                    {
                        configBuilder.AddCommandLine(args);
                    }
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<MetaClusterClientBuilder>();
                })
                .UseSerilog((hostContext, logging) =>
                {
                  logging.ReadFrom
                     .Configuration(hostContext.Configuration)
                     .Enrich.WithThreadId();
                })
                .UseStartup<Startup>()
                .Build();
        }
    }
}