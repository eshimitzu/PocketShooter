using System;
using System.Collections.Generic;
using System.Net;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public class GameRoomData
    {
        public GameRoomData()
        {
            // TODO: consider to use UTC everywere where
            // TOOD: consider using NodaTime for proper support of date-times as of now and all future events
            StartedAt = DateTime.MinValue;
            Players = new HashSet<Guid>();
            Bots = new HashSet<Guid>();
        }

        public IPEndPoint ServerAddress { get; set; }

        public Guid RoomId { get; set; }

        public DateTime StartedAt { get; set; }

        public bool IsStarted => StartedAt != DateTime.MinValue;

        public ISet<Guid> Players { get; private set; }

        public ISet<Guid> Bots { get; private set; }

        public int ParticipantsCount => Players.Count + Bots.Count;

        public MatchType MatchType { get; set; }

        public int DesiredBots { get; set; }

        public int InitialRealPlayers { get; set; }

        /// <summary>
        /// Room was rented for new game, but players may not joined yet.
        /// </summary>
        public bool IsRented { get; set; }

        public MapNames Map {get; set;}
    }
}
