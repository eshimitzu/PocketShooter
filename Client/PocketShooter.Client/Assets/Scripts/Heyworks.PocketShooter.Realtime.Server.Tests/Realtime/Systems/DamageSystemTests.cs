using System;
using System.Collections.Generic;
using Collections.Pooled;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.TestUtils;
using Xunit;

namespace Heyworks.PocketShooter.Realtime.Systems.Tests
{
    public class DamageSystemTests
    {
        private class Game : IServerGame
        {
            public Game(IReadOnlyDictionary<EntityId, ServerPlayer> players)
            {
                Players = players;
            }

            public DominationMatchResult MatchResult { get; }

            public ConsumablesMatchState ConsumablesState { get; }

            public void RespawnTrooper(string nickname, EntityId id, TeamNo teamNo, TrooperInfo trooperInfo) => throw new NotImplementedException();
            public Team Team1 => throw new NotImplementedException();
            public Team Team2 => throw new NotImplementedException();
            public Team GetTeam(TeamNo number) => throw new NotImplementedException();

            public long EndTime { get; set; }

            public IReadOnlyDictionary<byte, Zone> Zones => throw new NotImplementedException();

            public ServerPlayer GetServerPlayer(EntityId id)
            {
                Assert.True(Players.ContainsKey(id));
                return Players[id];
            }

            IPlayer IGame.GetPlayer(EntityId id) => Players[id];

            public IReadOnlyDictionary<EntityId, ServerPlayer> Players { get; }

            public bool IsEnded { get; set; }
        }

        [Fact]
        public void GetDamage()
        {
            var gameRef = new DummyGameRef();
            TrooperInfo testTrooperInfo = InfoUtils.TestTrooperInfo;
            gameRef.Value.Players.Add(
                PlayerState.Create(1, "1", TeamNo.First, testTrooperInfo, new FpsTransformComponent()));
            gameRef.Value.Players.Add(
                PlayerState.Create(2, "1", TeamNo.Second, testTrooperInfo, new FpsTransformComponent()));
            var attacker = new ServerPlayer(new PlayerRef(gameRef, 0), testTrooperInfo, new ConsumablesPlayerState(5, 5));
            var attacked = new ServerPlayer(new PlayerRef(gameRef, 1), testTrooperInfo, new ConsumablesPlayerState(5, 5));
            var game = new Game(new Dictionary<EntityId, ServerPlayer> { { attacker.Id, attacker }, { attacked.Id, attacked } });
            var attack = new AttackCommandData(1, new PooledList<ShotInfo>(new[] { new ShotInfo(2, WeaponName.M16A4, false) }));
            var ok = new WeaponDamageSystem(attack).Execute(attacker, game);
            Assert.True(ok);
            new DamageSystem(new GameArmorInfo(0.5f, 1)).Execute(game);
            Assert.True(attacked.Health.Health < attacked.Health.MaxHealth);
        }

        [Fact]
        public void CanKill()
        {
            TrooperInfo testTrooperInfo = InfoUtils.TestTrooperInfo;
            var gameRef = new DummyGameRef();
            gameRef.Value.Players.Add(PlayerState.Create(1, "1", TeamNo.First, testTrooperInfo, new FpsTransformComponent()));
            gameRef.Value.Players.Add(PlayerState.Create(2, "1", TeamNo.Second, testTrooperInfo, new FpsTransformComponent()));
            var attacker = new ServerPlayer(new PlayerRef(gameRef, 0), testTrooperInfo, new ConsumablesPlayerState(5, 5));
            var attacked = new ServerPlayer(new PlayerRef(gameRef, 1), testTrooperInfo, new ConsumablesPlayerState(5, 5));
            var game = new Game(new Dictionary<EntityId, ServerPlayer> { { attacker.Id, attacker }, { attacked.Id, attacked } });
            var attack = new AttackCommandData(1, new PooledList<ShotInfo>(new[] { new ShotInfo(2, WeaponName.M16A4, true) }));
            var weaponDamageSystem = new WeaponDamageSystem(attack);
            var damageSystem = new DamageSystem(new GameArmorInfo(0.5f, 1));
            while (attacked.IsAlive)
            {
                weaponDamageSystem.Execute(attacker, game);
                damageSystem.Execute(game);
            }

            Assert.Equal(attacker.Id, ((IPlayer)attacked).ServerEvents.LastKiller);
        }

        private class Ticker : ITicker
        {
            private int tick = 1;

            public int Current => tick++;
        }
    }
}
