using System;
using System.Collections.Generic;
using System.Text;
using Heyworks.PocketShooter.Realtime.Data;
using NUnit.Framework;


namespace Heyworks.PocketShooter.Realtime.Serialization
{
    public class GameJoinedDataSerializerTests
    {
        [Test]
        public void Works()
        {
            var serializer = new GameJoinedDataSerializer();
            var modeInfo = new DominationModeInfo(10, 9000, 6, 10, 5, 1000, new GameArmorInfo(1, 1),
                new DominationMapInfo(
                    new[] { new TeamInfo(TeamNo.First, new[] { new SpawnPointInfo(1, 2, 3.2f, 1.2f) }) },
                    new[] { new DominationZoneInfo(0, 1.5f, 1.6f, 2.3f, 5.6f) }));

            var itemsMetaInfo = new ItemsMetaInfo(WeaponName.M16A4, 10, HelmetName.Helmet2, 20, ArmorName.Armor3, 30);
            var trooperMetaInfo = new TrooperMetaInfo
            {
                Class = TrooperClass.Rambo,
                Grade = Grade.Uncommon,
                Level = 1,
                MaxLevel = 10,
                Health = 666,
                Armor = 66,
                Movement = 5.0,
                Power = 100,
                MaxPotentialPower = 1000,
                ItemsInfo = itemsMetaInfo,
            };

            var playerInfo = new PlayerInfo(Guid.NewGuid(), "player", new[] { new TrooperInfo(trooperMetaInfo) { MaxHealth = 666 } }, new ConsumablesInfo(5, 5));
            var input = new GameJoinedData(modeInfo, Guid.NewGuid(), TeamNo.Second, playerInfo, new EntityId(7), -1, -3);
            var data = serializer.Serialize(input);
            var output = serializer.Deserialize(data);
            Assert.AreEqual(input.RoomId, output.RoomId);
            Assert.AreEqual(input.TeamNo, output.TeamNo);
            Assert.AreEqual(input.ModeInfo.CheckInterval, output.ModeInfo.CheckInterval);
            Assert.AreEqual(input.ModeInfo.Map.Teams[0].Number, output.ModeInfo.Map.Teams[0].Number);
            Assert.AreEqual(input.ModeInfo.Map.Teams[0].SpawnPoints[0].Z, output.ModeInfo.Map.Teams[0].SpawnPoints[0].Z);
            Assert.AreEqual(input.ModeInfo.Map.Zones[0].Y, output.ModeInfo.Map.Zones[0].Y);
            Assert.AreEqual(input.PlayerInfo.Id, output.PlayerInfo.Id);
            Assert.AreEqual(input.PlayerInfo.Troopers[0].Class, output.PlayerInfo.Troopers[0].Class);
            Assert.AreEqual(input.PlayerInfo.Troopers[0].MetaInfo.Class, output.PlayerInfo.Troopers[0].MetaInfo.Class);
            Assert.AreEqual(input.PlayerInfo.Troopers[0].MetaInfo.Grade, output.PlayerInfo.Troopers[0].MetaInfo.Grade);
            Assert.AreEqual(input.PlayerInfo.Troopers[0].MetaInfo.Level, output.PlayerInfo.Troopers[0].MetaInfo.Level);
            Assert.AreEqual(input.PlayerInfo.Troopers[0].MetaInfo.Movement, output.PlayerInfo.Troopers[0].MetaInfo.Movement);
            Assert.AreEqual(input.PlayerInfo.Troopers[0].MetaInfo.ItemsInfo.ArmorName, output.PlayerInfo.Troopers[0].MetaInfo.ItemsInfo.ArmorName);
            Assert.AreEqual(input.PlayerInfo.Troopers[0].MetaInfo.ItemsInfo.ArmorPower, output.PlayerInfo.Troopers[0].MetaInfo.ItemsInfo.ArmorPower);
            Assert.AreEqual(input.PlayerInfo.Troopers[0].MaxHealth, output.PlayerInfo.Troopers[0].MaxHealth);
        }
    }
}
