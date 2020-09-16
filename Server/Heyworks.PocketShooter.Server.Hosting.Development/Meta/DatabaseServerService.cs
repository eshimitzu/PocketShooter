using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Heyworks.PocketShooter.Hosting
{
    // kills and runs local Mongo Database (mongod from specific path)
    // would run in Docker in Enterprise or in real development
    public class DatabaseServerService : IHostedService
    {
        private Process process;
        private IHostingEnvironment env;

        public DatabaseServerService(IHostingEnvironment env)
        {
            this.env = env;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            String mongoVersion, pathToDaemon;
            GetLatestMongo(out mongoVersion, out pathToDaemon);
            KillRunningMongo(pathToDaemon);
            StartNewMongo(mongoVersion, pathToDaemon);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            process.Kill();
            return Task.CompletedTask;
        }

        private void StartNewMongo(String mongoVersion, String pathToDaemon)
        {
            var yaml = new YamlDotNet.Serialization.Deserializer();
            var path = Path.Combine(env.ContentRootPath, $"mongod.{env.GetName().ToLower()}.cfg");
            using (var streamReader= new StreamReader(new FileStream(path, FileMode.Open)))
            {                
                var mongoConf = (Dictionary<object, object>) yaml.Deserialize(streamReader);
                var logPath = (string)(mongoConf["systemLog"] as Dictionary<object,object>)["path"];
                var dataDirectory = (string)(mongoConf["storage"] as Dictionary<object,object>)["dbPath"];
                var logs = Path.Combine(mongoVersion, Path.GetDirectoryName(logPath));
                if (!Directory.Exists(logs))
                    Directory.CreateDirectory(logs);

                var data = Path.Combine(mongoVersion, dataDirectory);
                if (!Directory.Exists(data))
                    Directory.CreateDirectory(data);                 
            }          
    
            var args = $" --config {path}";
            var mongoStart = new ProcessStartInfo(pathToDaemon, args) { WorkingDirectory = mongoVersion };
            process = Process.Start(mongoStart);
            Thread.Sleep(2000); // TODO: ping mongo with unit-identity query
        }

        private void KillRunningMongo(String pathToDaemon)
        {
            var running = Process.GetProcessesByName("mongod").Where(x => x.MainModule.FileName == pathToDaemon);
            foreach (var run in running)
                run.Kill();
        }

        private void GetLatestMongo(out String mongoVersion, out String pathToDaemon)
        {
            var pathToMongo = Path.GetFullPath("../../../../mongo");
            // latest
            mongoVersion = Directory.GetDirectories(pathToMongo).OrderByDescending(x => x).FirstOrDefault();
            if (mongoVersion == null)
                Throw.InvalidOperation($"Mongo was not found in {pathToMongo}");

            pathToDaemon = Path.Combine(mongoVersion, "bin", "mongod.exe");
        }
    }
}