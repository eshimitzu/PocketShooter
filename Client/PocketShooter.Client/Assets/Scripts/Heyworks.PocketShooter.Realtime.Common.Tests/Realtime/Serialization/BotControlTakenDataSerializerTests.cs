using System;
using System.Collections.Generic;
using System.Text;
using Heyworks.PocketShooter.Realtime.Data;
using NUnit.Framework;


namespace Heyworks.PocketShooter.Realtime.Serialization
{
    public class BotControlTakenDataSerializerTests
    {
        [Test]
        public void Works()
        {
            var serializer = new BotControlTakenDataSerializer();
            var botId = Guid.NewGuid();
            var trooperMetaInfo = new TrooperMetaInfo
            {
                Class = TrooperClass.Sniper
            };
            var playerInfo = new PlayerInfo(botId, "bot",  new[] { new TrooperInfo(trooperMetaInfo) { MaxHealth = 666, Weapon = new WarmingUpWeaponInfo{ WarmingSpeed = -1} } }, new ConsumablesInfo(5, 5));
            var input = new BotControlTakenData(playerInfo, TeamNo.Second, new EntityId(7));
            var data = serializer.Serialize(input);
            var output = serializer.Deserialize(data);
            Assert.AreEqual(input.BotInfo.Troopers[0].MaxHealth, output.BotInfo.Troopers[0].MaxHealth);
            Assert.AreEqual((input.BotInfo.Troopers[0].Weapon as WarmingUpWeaponInfo).WarmingSpeed, (output.BotInfo.Troopers[0].Weapon as WarmingUpWeaponInfo).WarmingSpeed);
            Assert.AreEqual(input.TeamNo, output.TeamNo);
        }
    }
}
