using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MoreLinq;
using Orleans;
using Orleans.Concurrency;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public class GameRoomsProviderGrain : Grain, IGameRoomsPublisherGrain, IGameRoomsProviderGrain
    {
        private List<GameRoomData> rooms;

        private readonly ILogger<GameRoomsProviderGrain> logger;

        public GameRoomsProviderGrain(ILogger<GameRoomsProviderGrain> logger)
        {
            this.rooms = new List<GameRoomData>();
            this.logger = logger;
        }

        public Task<GameRoomData> RentRoom(MatchType requestedMatch, MapNames map)
        {
            var room = GetRoom(requestedMatch, map);
            room.IsRented = true;
            return Task.FromResult(room);
        }

        private GameRoomData GetRoom(MatchType requestedMatch, MapNames map)
        {
            try
            {
                logger.LogDebug("Having {count} rooms to choose", rooms.Count);
                switch (requestedMatch)
                {
                    case MatchType.DominationBots:
                        return rooms.First(_ => !_.IsStarted && !_.IsRented);
                    case MatchType.DominationFast:
                        // TODO: take 10 from config
                        return rooms.Where(_ => _.MatchType == MatchType.DominationFast)
                                    .Where(_ => (_.Map == map && _.ParticipantsCount < 10) || _.ParticipantsCount == 0)
                                    .MaxBy(x => x.ParticipantsCount).First();
                    default:
                        return rooms.First(_ => !_.IsStarted && !_.IsRented);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get room for {requestedMatch}. See inner exception for detail.", ex);
            }
        }

        public Task PublishRoomData(Immutable<List<GameRoomData>> roomData)
        {
            logger.LogDebug("Updated with {count} rooms", roomData.Value.Count);
            foreach (var serverGroup in roomData.Value.GroupBy(item => item.ServerAddress))
            {
                var removed = rooms.RemoveAll(item => item.ServerAddress.Equals(serverGroup.Key));
                rooms.AddRange(serverGroup);

                logger.LogDebug(
                    $"Game rooms data updated. Current rooms:{Environment.NewLine}{{roomsInfo}}",
                    string.Join(
                        Environment.NewLine,
                        rooms.Select(item => (
                        $"Server: {item.ServerAddress}",
                        $"Room: {item.RoomId}",
                        $"Players: [{string.Join(",", item.Players)}]",
                        $"Bots: [{string.Join(",", item.Bots)}]",
                        $"Start Time: {item.StartedAt}"))));
            }

            // simplistic form to ensure we chose rooms randomly from any of attached realtime servers
            rooms = rooms.Shuffle().ToList();

            return Task.CompletedTask;
        }
    }
}