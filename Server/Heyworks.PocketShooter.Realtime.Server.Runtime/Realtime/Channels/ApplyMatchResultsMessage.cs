using System.Collections.Generic;

namespace Heyworks.PocketShooter.Realtime.Channels
{
    internal sealed class ApplyMatchResultsMessage : IManagementMessage
    {
        public ApplyMatchResultsMessage(RoomId roomId, IReadOnlyList<PlayerMatchResults> matchResults)
        {
            RoomId = roomId;
            MatchResults = matchResults;
        }

        public RoomId RoomId { get; }

        public IReadOnlyList<PlayerMatchResults> MatchResults { get; }
    }
}