using System;
using System.Collections.Generic;
using System.Linq;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.TestUtils;
using Xunit;
using Xunit.Abstractions;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public class ZoneCaptureSystemTests
    {
        private readonly ITestOutputHelper testOutputHelper;

        public ZoneCaptureSystemTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        private class GameMock : IServerGame
        {
            private Dictionary<byte, Zone> zones;
            public DominationMatchResult MatchResult { get; }

            public ConsumablesMatchState ConsumablesState { get; }

            public void RespawnTrooper(string nickname, EntityId id, TeamNo teamNo, TrooperInfo trooperInfo) => throw new NotImplementedException();

            public Team Team1 { get; }
            public Team Team2 { get; }

            public IPlayer GetPlayer(EntityId id) => throw new NotImplementedException();

            public Team GetTeam(TeamNo number)
            {
                switch (number)
                {
                    case TeamNo.First:
                        return Team1;
                    case TeamNo.Second:
                        return Team2;
                    default:
                        throw new ArgumentException($"Team number must be specified. TeamNo = {number.ToString()}");
                }
            }

            public IReadOnlyDictionary<byte, Zone> Zones => zones;

            public IReadOnlyDictionary<EntityId, ServerPlayer> Players { get; }

            public ServerPlayer GetServerPlayer(EntityId id) => throw new NotImplementedException();

            public bool IsEnded { get; set; }

            public long EndTime { get; set; }

            public DominationModeInfo DominationModeInfo { get; }

            public GameMock()
            {
                var zonesInfo = new[]
                {
                    new DominationZoneInfo(1,1,0,1,1),
                    new DominationZoneInfo(2,3,0,1,1),
                    new DominationZoneInfo(3,2,0,3,1),
                };
                DominationModeInfo = InfoUtils.TestDominationModeInfo(zonesInfo);
                TrooperInfo testTrooperInfo = InfoUtils.TestTrooperInfo;

                var gameRef = new DummyGameRef();

                gameRef.Value.Players.Add(PlayerState.Create(1, "1", TeamNo.First, testTrooperInfo, new FpsTransformComponent((1, 0, 0), 0, 0)));
                gameRef.Value.Players.Add(PlayerState.Create(2, "1", TeamNo.First, testTrooperInfo, new FpsTransformComponent((2, 0, 0), 0, 0)));
                gameRef.Value.Players.Add(PlayerState.Create(3, "1", TeamNo.First, testTrooperInfo, new FpsTransformComponent((3, 0, 0), 0, 0)));
                gameRef.Value.Players.Add(PlayerState.Create(4, "1", TeamNo.Second, testTrooperInfo, new FpsTransformComponent((0, 0, 1), 0, 0)));
                gameRef.Value.Players.Add(PlayerState.Create(5, "1", TeamNo.Second, testTrooperInfo, new FpsTransformComponent((0, 0, 2), 0, 0)));
                gameRef.Value.Players.Add(PlayerState.Create(6, "1", TeamNo.Second, testTrooperInfo, new FpsTransformComponent((0, 0, 3), 0, 0)));
                Players = gameRef.Value.Players.Select(
                            (x,i) =>
                            {
                                var id = (EntityId)(i+1);
                                return new KeyValuePair<EntityId, ServerPlayer>(id, new ServerPlayer(new PlayerRef(gameRef, (byte)i), testTrooperInfo, new ConsumablesPlayerState(5, 5)));
                            }).ToDictionary(x=>x.Key, x=> x.Value);
              
                Team1 = new Team(new TeamRefMock(gameRef.Value.Team1), DominationModeInfo.Map.Teams[0]);
                Team2 = new Team(new TeamRefMock(gameRef.Value.Team2), DominationModeInfo.Map.Teams[1]);
                Team1.State.Score = DominationModeInfo.WinScore;
                Team2.State.Score = DominationModeInfo.WinScore;
                zones = new Dictionary<byte, Zone>
                {
                    { 1, new Zone(new ZoneRefMock(gameRef.Value.Zones[0]), DominationModeInfo.Map.Zones[0]) }, // |O O| zones placement
                    { 2, new Zone(new ZoneRefMock(gameRef.Value.Zones[1]), DominationModeInfo.Map.Zones[1]) }, // | O |
                    { 3, new Zone(new ZoneRefMock(gameRef.Value.Zones[2]), DominationModeInfo.Map.Zones[2]) },
                };
            }
        }

        public class ZoneRefMock : IRef<ZoneState>
        {
            private ZoneState state;

            public ZoneRefMock(ZoneState state)
            {
                this.state = state;
            }

            public ref ZoneState Value => ref state;
        }

        public class TeamRefMock : IRef<TeamState>
        {
            private TeamState state;

            public TeamRefMock(TeamState state)
            {
                this.state = state;
            }
            
            public ref TeamState Value => ref state;
        }

        private class Ticks : ITicker
        {
            private int tick = 1;

            public int Current => tick++;
        }

        [Fact]
        public void RandomMovementTest()
        {
            var game = new GameMock();
            var modeInfo = game.DominationModeInfo;

            Random r = new Random(DateTime.Now.Millisecond);
            var zoneCaptureSystem = new ZoneCaptureSystem(new Ticks(), modeInfo.CheckInterval, modeInfo.TimeplayersToCapture);

            for (int i = 0; i < 1000; i++)
            {
                foreach (var player in game.Players.Values)
                {
                    var transform = new FpsTransformComponent(((float)(r.NextDouble() * 4), 0, (float)(r.NextDouble() * 4)), 0, 0);
                    new MoveSystem(new MoveCommandData(
                            player.Id, transform))
                        .Execute(player);

                    zoneCaptureSystem.Execute(game);
                }

                foreach (Zone zone in game.Zones.Values)
                {
                    if (zone.State.Captured)
                    {
                        Assert.True(zone.State.Progress > 0);
                        Assert.True(zone.State.OwnerTeam != TeamNo.None);
                    }
                    else
                    {
                        Assert.True(zone.State.Progress < game.DominationModeInfo.TimeplayersToCapture);
                    }

                    if (zone.State.OwnerTeam == TeamNo.None)
                    {
                        Assert.Equal(0, zone.State.Progress);
                        Assert.True(!zone.State.Captured);
                    }
                    else
                    {
                        Assert.True(zone.State.Progress > 0);
                    }
                }
            }

            double s1 = game.Team1.State.Score;
            double s2 = game.Team2.State.Score;
            testOutputHelper.WriteLine($"Team 1: {s1}");
            testOutputHelper.WriteLine($"Team 2: {s2}");

            double part = s1 / s2;
            Assert.True(part > 0.5 && part < 2);
        }

        [Fact]
        public void ZoneCaptureTest()
        {
            var game = new GameMock();
            var modeInfo = game.DominationModeInfo;

            var zone = game.Zones[1];
            var player = game.Players[1];
            var zoneCaptureSystem = new ZoneCaptureSystem(new Ticks(), modeInfo.CheckInterval, modeInfo.TimeplayersToCapture);
            var transform = new FpsTransformComponent((zone.ZoneInfo.X, 0, zone.ZoneInfo.Z), 0, 0);
            new MoveSystem(new MoveCommandData(player.Id, transform))
                .Execute(player);

            for (int i = 0; i < game.DominationModeInfo.TimeplayersToCapture; i++)
            {
                zoneCaptureSystem.Execute(game);
            }

            Assert.True(zone.State.Captured);
            Assert.Equal(game.DominationModeInfo.TimeplayersToCapture, zone.State.Progress);
            Assert.Equal(player.Team, zone.State.OwnerTeam);
            Assert.Equal(game.DominationModeInfo.WinScore + 1, game.Team1.State.Score);
        }

        [Fact]
        public void MultipleZonesCaptureTest()
        {
            var game = new GameMock();
            var modeInfo = game.DominationModeInfo;

            var zone1 = game.Zones[1];
            var zone2 = game.Zones[2];
            var zone3 = game.Zones[3];
            var player1 = game.Players[1];
            var player2 = game.Players[2];
            var player3 = game.Players[3];
            Assert.Equal(player1.Team, player2.Team);
            Assert.Equal(player1.Team, player3.Team);

            var zoneCaptureSystem = new ZoneCaptureSystem(new Ticks(), modeInfo.CheckInterval, modeInfo.TimeplayersToCapture);
            var transform1 = new FpsTransformComponent((zone1.ZoneInfo.X, 0, zone1.ZoneInfo.Z), 0, 0);
            new MoveSystem(new MoveCommandData(player1.Id, transform1)).Execute(player1);
            var transform2 = new FpsTransformComponent((zone2.ZoneInfo.X, 0, zone2.ZoneInfo.Z), 0, 0);
            new MoveSystem(new MoveCommandData(player2.Id, transform2)).Execute(player2);
            var transform3 = new FpsTransformComponent((zone3.ZoneInfo.X, 0, zone3.ZoneInfo.Z), 0, 0);
            new MoveSystem(new MoveCommandData(player3.Id, transform3)).Execute(player3);

            for (int i = 0; i < game.DominationModeInfo.TimeplayersToCapture; i++)
            {
                zoneCaptureSystem.Execute(game);
            }

            Assert.Equal(game.DominationModeInfo.WinScore + 3 * 3, game.Team1.State.Score);
        }

        [Fact]
        public void OutOfZoneOrDeadTest()
        {
            var game = new GameMock();
            var modeInfo = game.DominationModeInfo;

            var zone = game.Zones[1];
            var player = game.Players[1];
            var playerEnemy = game.Players[4];
            var zoneCaptureSystem = new ZoneCaptureSystem(new Ticks(), modeInfo.CheckInterval, modeInfo.TimeplayersToCapture);
            var transform = new FpsTransformComponent((zone.ZoneInfo.X, 0, zone.ZoneInfo.Z), 0, 0);
            new MoveSystem(new MoveCommandData(player.Id, transform)).Execute(player);
            var c = game.DominationModeInfo;

            for (int i = 0; i < c.TimeplayersToCapture - 1; i++)
            {
                zoneCaptureSystem.Execute(game);
            }

            Assert.True(!zone.State.Captured);
            Assert.Equal(c.TimeplayersToCapture - 1, zone.State.Progress);
            Assert.Equal(player.Team, zone.State.OwnerTeam);
            Assert.Equal(c.WinScore - 0, game.Team2.State.Score);

            var transform2 = new FpsTransformComponent((zone.ZoneInfo.X, 0, zone.ZoneInfo.Z), 0, 0);
            new MoveSystem(new MoveCommandData(playerEnemy.Id, transform2)).Execute(playerEnemy);
            zoneCaptureSystem.Execute(game);

            Assert.True(!zone.State.Captured);
            Assert.Equal(c.TimeplayersToCapture - 1, zone.State.Progress);
            Assert.Equal(player.Team, zone.State.OwnerTeam);
            Assert.Equal(c.WinScore - 0, game.Team2.State.Score);

            new MoveSystem(new MoveCommandData(player.Id, new FpsTransformComponent((0, 0, 0), 0, 0))).Execute(player);
            zoneCaptureSystem.Execute(game);

            Assert.True(!zone.State.Captured);
            Assert.Equal(c.TimeplayersToCapture - 2, zone.State.Progress);
            Assert.Equal(player.Team, zone.State.OwnerTeam);
            Assert.Equal(c.WinScore - 0, game.Team2.State.Score);

            for (int i = 0; i < c.TimeplayersToCapture - 2; i++)
            {
                zoneCaptureSystem.Execute(game);
            }
            
            Assert.True(!zone.State.Captured);
            Assert.Equal(0, zone.State.Progress);
            Assert.Equal(TeamNo.None, zone.State.OwnerTeam);
            Assert.Equal(c.WinScore - 0, game.Team2.State.Score);

            
            for (int i = 0; i < c.TimeplayersToCapture - 1; i++)
            {
                zoneCaptureSystem.Execute(game);
            }

            Assert.True(!zone.State.Captured);
            Assert.Equal(c.TimeplayersToCapture - 1, zone.State.Progress);
            Assert.Equal(playerEnemy.Team, zone.State.OwnerTeam);
            Assert.Equal(c.WinScore - 0, game.Team1.State.Score);

            playerEnemy.Health = new HealthComponent { MaxHealth = 10, Health = 0 };
            zoneCaptureSystem.Execute(game);

            Assert.True(!zone.State.Captured);
            Assert.Equal(c.TimeplayersToCapture - 2, zone.State.Progress);
            Assert.Equal(playerEnemy.Team, zone.State.OwnerTeam);
            Assert.Equal(c.WinScore - 0, game.Team1.State.Score);
        }

        [Fact]
        public void ZoneCaptureYAxisTest()
        {
            var game = new GameMock();
            var modeInfo = game.DominationModeInfo;

            Random r = new Random(DateTime.Now.Millisecond);
            var zone = game.Zones[1];
            var player = game.Players[1];
            var zoneCaptureSystem = new ZoneCaptureSystem(new Ticks(), modeInfo.CheckInterval, modeInfo.TimeplayersToCapture);

            for (int i = 0; i < game.DominationModeInfo.TimeplayersToCapture; i++)
            {
                var transform = new FpsTransformComponent((zone.ZoneInfo.X, r.Next(100000), zone.ZoneInfo.Z), 0, 0);
                new MoveSystem(new MoveCommandData(player.Id, transform)).Execute(player);
                zoneCaptureSystem.Execute(game);
            }

            Assert.True(zone.State.Captured);
            Assert.Equal(game.DominationModeInfo.TimeplayersToCapture, zone.State.Progress);
            Assert.Equal(player.Team, zone.State.OwnerTeam);
            Assert.Equal(game.DominationModeInfo.WinScore + 1, game.Team1.State.Score);
        }

        [Fact]
        public void NotInZoneTest()
        {
            var game = new GameMock();
            var modeInfo = game.DominationModeInfo;

            var p4 = game.Players[4];
            Random r = new Random(DateTime.Now.Millisecond);
            var zoneCaptureSystem = new ZoneCaptureSystem(new Ticks(), modeInfo.CheckInterval, modeInfo.TimeplayersToCapture);

            for (int i = 0; i < 100; i++)
            {
                var x = (float)(r.NextDouble() * 8 - 4);
                var transform = new FpsTransformComponent((x, 0, (float)(r.NextDouble() * 4) * Math.Sign(x) * -1), 0, 0);
                new MoveSystem(new MoveCommandData(p4.Id, transform)).Execute(p4);

                zoneCaptureSystem.Execute(game);
            }

            double s2 = game.Team2.State.Score;
            Assert.Equal(game.DominationModeInfo.WinScore, s2);
        }
    }
}