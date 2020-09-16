using System.Linq;
using System.IO;
using System;
using System.Net;
using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Communication;
using UniRx;
using NUnit.Framework;
using Heyworks.PocketShooter.Meta.Serialization;
using Microsoft.Extensions.Logging.Abstractions;
using Heyworks.PocketShooter.Realtime.Connection;
using Heyworks.PocketShooter.Realtime;
using static Heyworks.PocketShooter.Tests.FuncBoolExtensions;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Configuration;
using Heyworks.PocketShooter.Realtime.Entities;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Heyworks.PocketShooter.Tests
{
    public static class Extensions
    {
        public static bool WaitAll(this IEnumerable<Task> self, TimeSpan timeout)
        {
            var _ = self.ToArray();
            return Task.WaitAll(_, timeout);
        }
    }

    [TestFixture]
    public class GameHubTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        [TestCase(1, 30)]
        [Category(TestCategory.Integration)]
        public async Task NewDeviceMatchMakeAndMove(int numberOfUsers, ushort countOfSteps)
        {
            var setup = Environment.GetEnvironmentVariable("POCKET_SHOOTER_META");
            await DoGamePlay(numberOfUsers, countOfSteps, setup, Console.Out);
        }

        [Test]
        [TestCase(20, 3000)]
        [Category(TestCategory.Stress)]
        public async Task ManyUsersMatchMake(int numberOfUsers, ushort countOfSteps)
        {
            var setup = Environment.GetEnvironmentVariable("POCKET_SHOOTER_META");
            await DoGamePlay(numberOfUsers, countOfSteps, setup, Console.Out);
        }


        public async Task<(GameStateController realtime, Room room)> PlayingRoom(IPEndPoint realtime, PlayerId id, Context user)
        {
            Room room = null;
            Game game = null;
            IClientPlayer player = null;
            var stateController = new GameStateController();
            stateController.StateChanged += state =>
            {
                user.Out.WriteLine(state);
                if (state is GameRunningState runningState)
                {

                    room = runningState.Room;
                    room.GameStarted.Subscribe(localGame =>
                    {
                        game = localGame;
                        game.LocalPlayerSpawned.Subscribe(spawned =>
                        {
                            player = spawned.ClientPlayer;
                        });
                    });
                }
            };

            // TODO: plug config from file
            var entryState = new GameEntryState(stateController, PhotonConnection.CreateDefault(realtime), new RealtimeConfiguration(), id);
            stateController.SetCurrentState(entryState);

            await Poll(() =>
            {
                stateController.UpdateCurrentState();
                return room != null;
            }, TimeSpan.FromSeconds(10), "Timeout of waiting joining me into room");


            await Poll(() =>
            {
                stateController.UpdateCurrentState();
                return game != null;
            }, TimeSpan.FromSeconds(10), "Timeout of waiting game in the room");

            room.LocalPlayerSimulation.AddCommand(new SpawnTrooperCommandData(TrooperClass.Rambo));

            await Poll(() =>
            {
                stateController.UpdateCurrentState();
                return player != null;
            }, TimeSpan.FromSeconds(10), "Timeout of waiting trooper spawned");


            return (stateController, room);
        }


        private async Task DoGamePlay(int numberOfUsers, ushort countOfSteps, string setup, TextWriter output)
        {
            var tasks = Enumerable
                .Range(1, numberOfUsers)
                .Select(_ => new Context { meta = setup, id = _, totalTicks = countOfSteps, Out = output, network = 64 })
                .Select(_ =>
                {
                    Func<Task> __ = async () =>
                    {
                        var (realtime, room) = await DoMetaStuff(_);
                        await MoveAndWait(realtime, room, _);
                    };

                    return __;
                });

            var runs = tasks
                      .Select(Task.Run)
                      .WaitAll(TimeSpan.FromSeconds(30) + TimeSpan.FromMilliseconds(2 * countOfSteps * Constants.TickIntervalMs));
        }

        public async Task<(GameStateController realtime, Room room)> DoMetaStuff(Context user)
        {
            // TODO: may use specflow to scale test and make them more readable
            // TODO: add logs to report issues and progress, serilog or nunit native

            var (meta, events) = await ConnectPlayer(user.meta);
            user.Out.WriteLine($"Before ConnectAsync");
            await meta.ConnectAsync();
            user.Out.WriteLine($"Before MakeMatchAsync");
            await meta.MakeMatchAsync(new MatchRequest(MatchType.Domination));

            var realtimeServer = await events.Maked.Task.ConfigureAwait(false);
            var metaGame = await events.GotState.Task.ConfigureAwait(false);
            user.Out.WriteLine($"{user.id} got match");
            var (realtime, room) = await PlayingRoom(realtimeServer.ServerAddress, metaGame.Player.Id, user);
            return (realtime, room);
        }

        private async Task MoveAndWait(GameStateController realtime, Room room, Context user)
        {
            user.Out.WriteLine("Starting move " + user.id);
            var originalLocation = (room.Game.LocalPlayer as IClientPlayer).Transform.Position;
            for (int steps = 0; steps < user.totalTicks; steps++)
            {
                var polling = originalLocation + (steps % 2 == 0 ? 1 : -1);
                await Poll(async () =>
                {
                    Move(room, polling);
                    realtime.UpdateCurrentState();
                    await Task.Delay(Constants.TickIntervalMs);
                    var newState = room.Game.LocalPlayer.Ref;
                    return newState.Value.Transform.Position.NearEquals(polling);
                }, TimeSpan.FromMilliseconds(user.network * Constants.TickIntervalMs * 2), "Failed to move player");

            }
        }

        private static void Move(Room room, Position polling)
        {
            var transform = new FpsTransformComponent(polling, 0, 0);
            var move = new MoveCommandData(room.Game.LocalPlayer.Id, transform);
            room.LocalPlayerSimulation.AddCommand(move);
        }

        private async Task<(IPEndPoint, PlayerId)> MatchMake(Task<(IGameHubClient Meta, GameHubObserver events)> task)
        {
            var (meta, events) = task.Result;
            await meta.MakeMatchAsync(new MatchRequest(MatchType.Domination));
            var realtime = await events.Maked.Task.ConfigureAwait(false);
            var metaGameState = await events.GotState.Task.ConfigureAwait(false);
            return (realtime.ServerAddress, metaGameState.Player.Id);
        }

        private static async Task<(IGameHubClient meta, GameHubObserver events)> ConnectPlayer(string metaServer)
        {
            var sync = new TaskSynchronizationContext();
            // TODO: setup real log to trace test execution
            var logger = new NullLoggerFactory();
#if !UNITY_5_3_OR_NEWER            
            MLog.Setup(logger);
#endif
            // TODO: path configs via path to files and setup meta and logger from these

            var appConfiguration = new Config(metaServer);
            var device = Guid.NewGuid().ToString();
            var dataSerializer = new JsonSerializer(new DefaultSerializerSettings());
            var connectionListener = new DummyConnectionListener();
            var web = new WebApiClient(appConfiguration.MetaServerAddress.Address, appConfiguration.Version, dataSerializer, logger, connectionListener);
            var login = new LoginRequest { DeviceId = device, ApplicationStore = ApplicationStoreName.Google, BundleId = appConfiguration.BundleId, ClientVersion = appConfiguration.Version, };

            var tokenProvider = new DeviceAccessTokenProvider(
                appConfiguration.MetaServerAddress.Address, appConfiguration.Version, login, dataSerializer, logger, connectionListener);

            var socket = new GameHubClient(
                appConfiguration.MetaServerAddress.Address, appConfiguration.Version, tokenProvider, sync, logger, connectionListener);

            var register = new RegisterRequest
            {
                ApplicationStore = ApplicationStoreName.Google,
                BundleId = appConfiguration.BundleId,
                ClientVersion = appConfiguration.Version,
                DeviceId = device,
                Country = "BY"
            };
            var result = await web.RegisterDevice(register);
            Assert.IsNull(result.Error);
            Assert.IsNotNull(result.Ok.Data.GameConfigVersion);
            var x = new GameHubObserver();
            socket.Subscribe(x);
            await socket.ConnectAsync();
            return (socket, x);
        }
    }
}