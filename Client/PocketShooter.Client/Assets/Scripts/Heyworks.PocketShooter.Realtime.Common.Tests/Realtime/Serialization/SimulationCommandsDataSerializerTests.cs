using System;
using Heyworks.PocketShooter.Realtime.Data;
using NUnit.Framework;

namespace Heyworks.PocketShooter.Realtime.Serialization
{
    public class SimulationCommandsDataSerializerTests
    {
        [Test]
        public void ShouldAddPingBeforeSerialize()
        {
            var serializer = new SimulationCommandsDataSerializer();
            Assert.Throws<InvalidOperationException>(() => serializer.Serialize());
        }

        [Test]
        public void ShouldAddPingBeforeAddCommand()
        {
            var serializer = new SimulationCommandsDataSerializer();
            Assert.Throws<InvalidOperationException>(() => serializer.Add(new SimulationCommandData(0, null)));
        }

        [Test]
        public void ManyPingsAndNoCommands()
        {
            var serializer = new SimulationCommandsDataSerializer();
            var random = new Random(42);
            var lastConfirmed = 0;

            for (int i = 0; i < 1024; i++)
            {
                lastConfirmed = Math.Max(i - 2 - random.Next(2), lastConfirmed);
                var possiblyStillConfirmed = Math.Max(i - random.Next(7), 0);
                serializer.AddPing(new SimulationMetaCommandData(i, lastConfirmed), possiblyStillConfirmed);
                var data = serializer.Serialize();
                var (ping,commands) = serializer.Deserialize(data);
                Assert.AreEqual(i, ping.Tick);
                Assert.AreEqual(lastConfirmed, ping.ConfirmedTick);
                Assert.AreEqual(0, commands.Count);
            }
        }

        [Test]
        public void SerializeAndDeserializeOnlyPing()
        {
            var serializer = new SimulationCommandsDataSerializer();
            serializer.AddPing(new SimulationMetaCommandData(5, 1), 2);
            var useMedKit = new ReloadCommandData(new EntityId(7));
            var data = serializer.Serialize();
            var (lastPing, commands) = serializer.Deserialize(data);
            Assert.AreEqual(5, lastPing.Tick);
            Assert.AreEqual(1, lastPing.ConfirmedTick);
        }

        [Test]
        public void CommandResendLimit()
        {
            var serializer = new SimulationCommandsDataSerializer();
            serializer.AddPing(new SimulationMetaCommandData(5, 1), 4);
            serializer.Add(new SimulationCommandData(5, new ReloadCommandData(new EntityId(1))));
            serializer.AddPing(new SimulationMetaCommandData(6, 1), 4);
            serializer.Add(new SimulationCommandData(6, new ReloadCommandData(new EntityId(1))));
            var data = serializer.Serialize();
            var (ping, commands) = serializer.Deserialize(data);
            Assert.AreEqual(2, commands.Count);
            serializer.AddPing(new SimulationMetaCommandData(7, 1), 4);
            serializer.Add(new SimulationCommandData(7, new ReloadCommandData(new EntityId(1))));
            serializer.AddPing(new SimulationMetaCommandData(8, 1), 4);
            serializer.Add(new SimulationCommandData(8, new ReloadCommandData(new EntityId(1))));
            data = serializer.Serialize();
            (ping, commands) = serializer.Deserialize(data);
            Assert.AreEqual(Constants.CommandsResendCountMax, commands.Count);
            serializer.AddPing(new SimulationMetaCommandData(9, 1), 4);
            serializer.Add(new SimulationCommandData(9, new ReloadCommandData(new EntityId(1))));
            data = serializer.Serialize();
            (ping, commands) = serializer.Deserialize(data);
            Assert.AreEqual(Constants.CommandsResendCountMax, commands.Count);
        }

        [Test]
        public void SerializeAndDeserialize()
        {
            var serializer = new SimulationCommandsDataSerializer();
            serializer.AddPing(new SimulationMetaCommandData(5, 1), 2);
            serializer.Add(new SimulationCommandData(5, new ReloadCommandData(new EntityId(7))));
            var data = serializer.Serialize();
            var (lastPing, commands) = serializer.Deserialize(data);
            Assert.AreEqual(5, lastPing.Tick);
            Assert.AreEqual(1, lastPing.ConfirmedTick);
            Assert.AreEqual(1, commands.Count);
            Assert.AreEqual(new EntityId(7), (commands[0].GameCommandData as ReloadCommandData).ActorId);
            Assert.AreEqual(5, commands[0].Tick);
        }

        [Test]
        public void SerializeAndDeserializeTwo()
        {
            var serializer = new SimulationCommandsDataSerializer();
            serializer.AddPing(new SimulationMetaCommandData(5, 1), 2);
            serializer.Add(new SimulationCommandData(5, new ReloadCommandData(new EntityId(7))));
            serializer.Add(new SimulationCommandData(5, new ReloadCommandData(new EntityId(9))));
            var data = serializer.Serialize();
            var (lastPing, commands) = serializer.Deserialize(data);
            Assert.AreEqual(5, lastPing.Tick);
            Assert.AreEqual(1, lastPing.ConfirmedTick);
            Assert.AreEqual(2, commands.Count);
            Assert.AreEqual(new EntityId(9), (commands[1].GameCommandData as ReloadCommandData).ActorId);
            Assert.AreEqual(5, commands[1].Tick);
        }

        [Test]
        public void AddWithOldPing()
        {
            var serializer = new SimulationCommandsDataSerializer();
            serializer.AddPing(new SimulationMetaCommandData(4, 0), 2);
            serializer.AddPing(new SimulationMetaCommandData(5, 1), 2);
            Assert.Throws<InvalidOperationException>(() => serializer.Add(new SimulationCommandData(4, new ReloadCommandData(new EntityId(7)))));
        }

        [Test]
        public void AddWithTwoPings()
        {
            var serializer = new SimulationCommandsDataSerializer();
            serializer.AddPing(new SimulationMetaCommandData(4, 0), 2);
            var useMedKit = new ReloadCommandData(new EntityId(7));
            serializer.Add(new SimulationCommandData(4, useMedKit));
            serializer.AddPing(new SimulationMetaCommandData(5, 1), 2);
            serializer.Add(new SimulationCommandData(5, useMedKit));
            var data = serializer.Serialize();
            var (lastPing, commands) = serializer.Deserialize(data);
            Assert.AreEqual(5, lastPing.Tick);
            Assert.AreEqual(1, lastPing.ConfirmedTick);
            Assert.AreEqual(2, commands.Count);
            Assert.AreEqual(5, commands[0].Tick);
            Assert.AreEqual(useMedKit.Code, commands[0].GameCommandData.Code);
            Assert.AreEqual(4, commands[1].Tick);
            Assert.AreEqual(useMedKit.Code, commands[1].GameCommandData.Code);
        }

        [Test]
        public void NotMoving()
        {
            var serializer = new SimulationCommandsDataSerializer();
            serializer.AddPing(new SimulationMetaCommandData(4, 0), 2);
            serializer.Add(new SimulationCommandData(4, new MoveCommandData(new EntityId(8), new FpsTransformComponent((1, 2, 1), 1, 1))));
            serializer.AddPing(new SimulationMetaCommandData(5, 1), 2);
            serializer.Add(new SimulationCommandData(5, new MoveCommandData(new EntityId(8), new FpsTransformComponent((1, 2, 1), 1, 1))));
            var data = serializer.Serialize();
            var (lastPing, commands) = serializer.Deserialize(data);
            Assert.AreEqual(1, commands.Count);
        }

        [Test]
        public void WarmUpOneTime()
        {
            var serializer = new SimulationCommandsDataSerializer();
            serializer.AddPing(new SimulationMetaCommandData(4, 0), 2);
            serializer.Add(new SimulationCommandData(4, new WarmingUpCommandData(new EntityId(8))));
            serializer.Add(new SimulationCommandData(4, new WarmingUpCommandData(new EntityId(8))));
            var data = serializer.Serialize();
            var (lastPing, commands) = serializer.Deserialize(data);
            Assert.AreEqual(1, commands.Count);
        }

        [Test]
        public void UseDifferentSkills()
        {
            var serializer = new SimulationCommandsDataSerializer();
            serializer.AddPing(new SimulationMetaCommandData(4, 0), 2);
            serializer.Add(new SimulationCommandData(4, new UseSkillCommandData(new EntityId(8), SkillName.Invisibility)));
            serializer.Add(new SimulationCommandData(4, new UseSkillCommandData(new EntityId(8), SkillName.Sprint)));
            var data = serializer.Serialize();
            var (lastPing, commands) = serializer.Deserialize(data);
            Assert.AreEqual(2, commands.Count);
        }

        [Test]
        public void PingCycle()
        {
            var serializer = new SimulationCommandsDataSerializer();
            for (var t = 0; t < 5; t++)
            {
                serializer.AddPing(new SimulationMetaCommandData(t, 0), t);
                var data = serializer.Serialize();
                var (lastPing, commands) = serializer.Deserialize(data);
                Assert.AreEqual(t, lastPing.Tick);
                Assert.AreEqual(0, lastPing.ConfirmedTick);
                Assert.AreEqual(0, commands.Count);
            }
        }

        [Test]
        public void CommandsCycle()
        {
            var serializer = new SimulationCommandsDataSerializer();
            for (var t = 0; t < 5; t++)
            {
                serializer.AddPing(new SimulationMetaCommandData(t, 0), t);
                serializer.Add(new SimulationCommandData(t, new ReloadCommandData(new EntityId(7))));
                serializer.Add(new SimulationCommandData(t, new MoveCommandData(new EntityId(8), new FpsTransformComponent((t, 2, 1), 1, 1))));
                var data = serializer.Serialize();
                var (lastPing, commands) = serializer.Deserialize(data);
                Assert.AreEqual(t, lastPing.Tick);
                Assert.AreEqual(0, lastPing.ConfirmedTick);
                Assert.AreEqual(2, commands.Count);
                Assert.AreEqual(t, commands[0].Tick);
                Assert.AreEqual(t, commands[1].Tick);
                Assert.True(commands[1].GameCommandData is MoveCommandData);
            }
        }

        [Test]
        public void AddWithTwoPingsButWithResendOnlyCurrent()
        {
            var serializer = new SimulationCommandsDataSerializer();
            serializer.AddPing(new SimulationMetaCommandData(4, 0), 4);
            serializer.Add(new SimulationCommandData(4, new ReloadCommandData(new EntityId(7))));
            serializer.AddPing(new SimulationMetaCommandData(5, 1), 5);
            serializer.Add(new SimulationCommandData(5, new ReloadCommandData(new EntityId(7))));
            var data = serializer.Serialize();
            var (lastPing, commands) = serializer.Deserialize(data);
            Assert.AreEqual(5, lastPing.Tick);
            Assert.AreEqual(1, lastPing.ConfirmedTick);
            Assert.AreEqual(1, commands.Count);
            Assert.AreEqual(5, commands[0].Tick);
        }

        [Test]
        public void MoveAttack()
        {
            var write = new SimulationCommandsDataSerializer();
            write.AddPing(new SimulationMetaCommandData(4, 0), 4);
            var transform = new FpsTransformComponent((1f, 2f, 3f), 4f, 5f);
            write.Add(new SimulationCommandData(4, new MoveCommandData(1, transform)));
            var attackSent = new AttackCommandData(1, new Collections.Pooled.PooledList<ShotInfo> { new ShotInfo(2, WeaponName.M16A4, true) });
            write.Add(new SimulationCommandData(4, attackSent));
            var data = write.Serialize();
            var read = new SimulationCommandsDataSerializer();
            var (lastPing, commands) = read.Deserialize(data);
            var move = (MoveCommandData)commands[0].GameCommandData;
            Assert.AreEqual(1f, move.Transform.Position.X, 2);
            Assert.AreEqual(2f, move.Transform.Position.Y, 2);
            Assert.AreEqual(3f, move.Transform.Position.Z, 2);
            Assert.AreEqual(4f, move.Transform.Yaw, 2);
            Assert.AreEqual(5f, move.Transform.Pitch, 2);

            var attack = (AttackCommandData)commands[1].GameCommandData;
            Assert.AreEqual(new EntityId(1), attack.ActorId);
            Assert.AreEqual(WeaponName.M16A4, attack.Shots[0].WeaponName);
            Assert.AreEqual(new EntityId(2), attack.Shots[0].AttackedId);
            Assert.True(attack.Shots[0].IsHeadshot);
        }
    }
}
