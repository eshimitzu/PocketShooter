using System.Runtime.CompilerServices;
using System;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using NUnit.Framework;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Meta.Serialization;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.Logging.Abstractions;

namespace Heyworks.PocketShooter.Meta.Configuration
{
    public class GameConfigProviderTests
    {
        private class NUNitHostingEnvironment : IHostingEnvironment, IFileProvider, IDirectoryContents, IFileInfo
        {
            public NUNitHostingEnvironment()
            {
                ContentRootFileProvider = this;
            }

            public string EnvironmentName { get; set; } = "NUnit";
            public string ApplicationName { get; set; } = nameof(GameConfigProviderTests);
            public string ContentRootPath { get; set; } = ".";
            public IFileProvider ContentRootFileProvider { get; set; }

            public bool Exists => true;

            public long Length => throw new NotImplementedException();

            public string PhysicalPath => throw new NotImplementedException();

            public string Name => "pocketshooter.json";

            public DateTimeOffset LastModified => throw new NotImplementedException();

            public bool IsDirectory => throw new NotImplementedException();

            public Stream CreateReadStream()
            {
                return File.OpenRead(@"D:\Heyworks\PocketShooter\Server\.gameconfigs\pocketshooter.json");
            }

            public IDirectoryContents GetDirectoryContents(string subpath) => this;

            public IEnumerator<IFileInfo> GetEnumerator()
            {
                yield return this;
            }

            public IFileInfo GetFileInfo(string subpath)
            {
                throw new NotImplementedException();
            }

            public IChangeToken Watch(string filter)
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        [Ignore("YGR. Runs only on my PC")]
        public void IfThereIsNoSettingsFolderThanThrows()
        {
            var config = new ServerGameConfig();
            var settings = new DefaultSerializerSettings();
            settings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

            var configSerialied = JsonConvert.SerializeObject(config, settings);
            File.WriteAllText(@"D:\Heyworks\PocketShooter\Server\.gameconfigs\pocketshooter.json", configSerialied);

            var hosting = new NUNitHostingEnvironment();
            var gameConfig = new GameConfigProvider(hosting, NullLoggerFactory.Instance.CreateLogger<GameConfigProvider>());
            var readed = gameConfig.ReadGameConfigs().Result;
            Assert.IsNotNull(readed.Single().Value.RealtimeConfig.DominationModeConfig);
        }
    }
}