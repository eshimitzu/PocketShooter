using System.Linq;
using Collections.Pooled;
using Heyworks.PocketShooter.Realtime.Data;
using NUnit.Framework;

namespace Heyworks.PocketShooter.Realtime.Serialization
{
    public class SimulationStateSerializerTests
    {
        [Test]
        public void SerializeTest()
        {
            var shots = new PooledList<ShotInfo>(new[] { new ShotInfo(2, WeaponName.M16A4, false) });
            var damages = new PooledList<DamageInfo>(new[] { new DamageInfo(1, new EntityRef(EntityType.Weapon, (byte)WeaponName.M16A4), DamageType.Normal, 1f) });
            var heals = new PooledList<HealInfo>(new[] { new HealInfo(HealType.MedKit, 10),});
            var remoteServerEvents = new RemoteServerEvents { LastKiller = 1 };
            var weaponForAll = new WeaponBaseComponent { State = WeaponState.Attacking, Name = WeaponName.Remington };
            var ownedWeapon = new WeaponConsumableComponent { AmmoInClip = 42 };
            var weapon = new WeaponComponents() { Consumable = ownedWeapon, Base = weaponForAll };
            var fpsTransform = new FpsTransformComponent { Position = (1.5f, 2.4f,3.12f), Yaw = 4.3456f, Pitch = 5.345f };

            var localPlayerState = new PlayerState(
                1,
                "1",
                TrooperClass.Rambo,
                TeamNo.First,
                fpsTransform,
                new HealthComponent { MaxHealth = 10.3f, Health = 10.3f },
                new ArmorComponent(),
                weapon,
                new SkillComponents(),
                new SkillComponents(),
                new SkillComponents(),
                new SkillComponents(),
                new SkillComponents(),
                new EffectComponents(),
                remoteServerEvents,
                shots,
                damages,
                heals);
            PlayerState other1 = default;
            PlayerState other2 = default;
            localPlayerState.Clone(ref other1);
            other1.Id = 10;
            other1.Team = TeamNo.First;
            localPlayerState.Clone(ref other2);
            other2.Id = 20;
            other2.Team = TeamNo.Second;

            var teamState = new TeamState(TeamNo.First, 100);
            var teamState2 = new TeamState(TeamNo.Second, 200);
            var zoneState = new ZoneState(1, TeamNo.First, 3, true);

            var players = new PooledList<PlayerState> { localPlayerState, other1, other2 };
            var zones = new[] { zoneState };

            var gameState = new GameState(teamState, teamState2, zones, players);
            SimulationState? dummy = null;

            var simulationState = new SimulationState(100001, 10, gameState);

            var serializer = new SimulationStateSerializer(Constants.BufferSize);
            var simulationStateSerialized = serializer.Serialize(in dummy, in simulationState);

            var simulationStateDeserialized = serializer.Deserialize(simulationStateSerialized).Value;
            var gameStateDeserialized = simulationStateDeserialized.GameState;

            Assert.AreEqual(simulationState.Tick, simulationStateDeserialized.Tick);
            Assert.AreEqual(simulationState.ServerInputBufferSize, simulationStateDeserialized.ServerInputBufferSize);
            Assert.AreEqual(gameState.Zones.Count(), gameStateDeserialized.Zones.Count());
            Assert.AreEqual(gameState.Zones[0], gameStateDeserialized.Zones[0]);
            Assert.AreEqual(gameState.Players.Count(), gameStateDeserialized.Players.Count());
            Assert.AreEqual(gameState.Players[0].Id, gameStateDeserialized.Players[0].Id);
            Assert.True(gameState.Players[0].Transform.NearEquals(gameStateDeserialized.Players[0].Transform));

            Assert.AreEqual(gameState.Players[0].Health, gameStateDeserialized.Players[0].Health);
            Assert.AreEqual(gameState.Players[0].Weapon, gameStateDeserialized.Players[0].Weapon);

            // events
            Assert.AreEqual(gameState.Players[0].ServerEvents.LastKiller, gameStateDeserialized.Players[0].ServerEvents.LastKiller);
            Assert.AreEqual(gameState.Players[0].Shots, gameStateDeserialized.Players[0].Shots);
            Assert.AreEqual(gameState.Players[0].Damages, gameStateDeserialized.Players[0].Damages);
            Assert.AreEqual(gameState.Players[0].Heals, gameStateDeserialized.Players[0].Heals);
            Assert.AreEqual(gameState.Team1.Number, teamState.Number);
            Assert.AreEqual(gameState.Team1.Number, teamState.Number);
            Assert.AreEqual(gameState.Team2.Score, teamState2.Score);
            Assert.AreEqual(gameState.Team2.Score, teamState2.Score);
            Assert.AreEqual(gameState.Zones[0].Id, zones[0].Id);
            Assert.AreEqual(gameState.Zones[0].OwnerTeam, zones[0].OwnerTeam);
            Assert.AreEqual(gameState.Zones[0].Captured, zones[0].Captured);
            Assert.AreEqual(gameState.Zones[0].Progress, zones[0].Progress);

            Assert.True(other1.Transform.NearEquals(gameStateDeserialized.Players[1].Transform));

            Assert.AreEqual(other1.Health, gameStateDeserialized.Players[1].Health);
            Assert.AreEqual(other1.Weapon, gameStateDeserialized.Players[1].Weapon);
        }
    }
}
