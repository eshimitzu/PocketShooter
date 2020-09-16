using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HealthChecks.Network;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using Orleans;
using System.Reflection.PortableExecutable;

namespace Heyworks.PocketShooter.Meta.Communication
{
    [AllowAnonymous]
    [Route("health/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private byte[] photonHeader = new byte[] { 0xff, 0xff, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x75, 0xfa, 0x36, 0x56, 0x02, 0xff, 0x01, 0x04, 0x00, 0x00, 0x00, 0x2c, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x04, 0xb0, 0x00, 0x00, 0x80, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x13, 0x88, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x02 };

        Process currentProcess = Process.GetCurrentProcess();
        private String approximateDeployTime;
        private string lastWriteTime;
        private IHostingEnvironment hosting;
        private string assemblyTimeDateStamp;
        private const string iso8601 = "yyyy-MM-ddTHH:mm:ss.fffzzz";
        private const string iso8601Utc = iso8601 + "Z";

#if DEBUG
        const string Build = "Debug";
#else
        const string Build = "Release";
#endif

        // avoids any other dependency to prevent failur
        public StatusController(IHostingEnvironment hosting)
        {
            var location = Assembly.GetExecutingAssembly().Location;
            var directoryLastWriteTime = Directory.GetLastWriteTimeUtc(Path.GetDirectoryName(location));
            var assemblyLastWriteTime = System.IO.File.GetLastWriteTimeUtc(location);
            lastWriteTime = (directoryLastWriteTime > assemblyLastWriteTime ? directoryLastWriteTime : assemblyLastWriteTime).ToString(iso8601Utc);

            using (var self = new PEReader(System.IO.File.OpenRead(location)))
                assemblyTimeDateStamp = DateTimeOffset.FromUnixTimeSeconds(self.PEHeaders.CoffHeader.TimeDateStamp).ToString("yyyy-MM-ddTHH:mm:ssZ");

            this.hosting = hosting;
        }

        // returns public static information safe to share
        // GET api/status
        [HttpGet]
        public ActionResult<JObject> Get()
        {
            var runningTime = DateTime.UtcNow - currentProcess.StartTime.ToUniversalTime();
            var status = new JObject
            {
                ["application"] = new JObject
                {
                    [nameof(hosting.ApplicationName)] = hosting.ApplicationName,
                    [nameof(hosting.EnvironmentName)] = hosting.EnvironmentName,
                },

                ["build"] = new JObject
                {
                    [nameof(Build)] = Build,
                    [nameof(ThisAssembly.Git.BaseTag)] = ThisAssembly.Git.BaseTag,
                    [nameof(ThisAssembly.Git.Commit)] = ThisAssembly.Git.Commit,
                    [nameof(assemblyTimeDateStamp)] = assemblyTimeDateStamp,
                },

                ["deployment"] = new JObject
                {
                    [nameof(lastWriteTime)] = lastWriteTime,
                    [nameof(runningTime)] = runningTime.ToString(),
                    [nameof(DateTime.Now)] = DateTime.Now.ToString(iso8601),
                    [nameof(DateTime.UtcNow)] = DateTime.UtcNow.ToString(iso8601Utc),
                }
            };

            return status;
        }

        [HttpGet("attached")]
        public async Task<JObject[]> GetAttachedRealtimeServers([FromServices]IClusterClient metaBack, [FromServices]IMemoryCache cache)
        {
            var cacheEntry = await
                cache.GetOrCreateAsync(nameof(GetAttachedRealtimeServers), async entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromSeconds(30));
                var grain = metaBack.GetGrain<IMatchMakingGrain>(Guid.Empty);
                var ipPort = await grain.GetRealtimeServers();

                var checks = ipPort.Select(async x =>
                {
                    var udpOptions = new UdpHealthCheckOptions();
                    udpOptions.AddHost(x.Address.ToString(), x.Port, photonHeader);
                    var udpCheck = new UdpHealthCheck(udpOptions);
                    var healthResult = await udpCheck.CheckHealthAsync(new HealthCheckContext { Registration = new HealthCheckRegistration("upd", udpCheck, null, null) });
                    return new JObject
                    {
                        [nameof(x.Address)] = x.Address.ToString(),
                        [nameof(x.Port)] = x.Port,
                        [nameof(healthResult.Status)] = healthResult.Status.ToString()
                    };
                })
                .ToArray();
                return await Task.WhenAll(checks);
            });

            return cacheEntry;
        }
    }
}
