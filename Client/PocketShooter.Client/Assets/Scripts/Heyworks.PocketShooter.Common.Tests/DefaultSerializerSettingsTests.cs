using NUnit.Framework;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Communication;
using Newtonsoft.Json;
using Heyworks.PocketShooter.Meta.Serialization;

namespace Tests
{
    [TestFixture]
    public class DefaultSerializerSettingsTests
    {
        [Test]
        public void TestJsonDotNet()
        {
            var x = JsonSerializer.Create(new DefaultSerializerSettings());
            var s = new MemoryStream();
            var tw = new StreamWriter(s);
            var input = new MatchMakingResultData(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 123), Heyworks.PocketShooter.MapNames.Mexico);
            x.Serialize(tw, input);
            tw.Flush();
            s.Position = 0;
            var result = x.Deserialize<MatchMakingResultData>(new JsonTextReader(new StreamReader(s)));
            Assert.AreEqual(input.ServerAddress.Address, result.ServerAddress.Address);
            Assert.AreEqual(input.ServerAddress.Port, result.ServerAddress.Port);
        }
    }
}