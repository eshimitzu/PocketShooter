using System;
using System.Text;
using Collections.Pooled;

namespace Heyworks.PocketShooter.Realtime.Data
{
    /// <summary>
    /// Represents a game state.
    /// </summary>
    public struct GameState : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameState"/> struct.
        /// </summary>
        /// <param name="team1">First team.</param>
        /// <param name="team2">Second Team.</param>
        /// <param name="zones">The game zones.</param>
        /// <param name="players">The player states.</param>
        public GameState(TeamState team1, TeamState team2, ZoneState[] zones, PooledList<PlayerState> players)
        {
            Team1 = team1;
            Team2 = team2;
            Zones = zones;
            Players = players;
        }

        public static GameState Create(DominationModeInfo modeInfo)
        {
            var zonesInfo = modeInfo.Map.Zones;
            var zones = new ZoneState[zonesInfo.Length];
            for (int i = 0; i < zonesInfo.Length; i++)
            {
                zones[i] = new ZoneState(zonesInfo[i].Id, TeamNo.None, 0, false);
            }

            var state = new GameState(
                new TeamState(TeamNo.First, 0),
                new TeamState(TeamNo.Second, 0),
                zones,
                new PooledList<PlayerState>(modeInfo.MaxPlayers));
            return state;
        }

        /// <summary>
        /// Gets the first team.
        /// </summary>
        public TeamState Team1;

        /// <summary>
        /// Gets the first team.
        /// </summary>
        public TeamState Team2;

        /// <summary>
        /// Gets the zones.
        /// </summary>
        public readonly ZoneState[] Zones;

        /// <summary>
        /// Gets the player states.
        /// </summary>
        public readonly PooledList<PlayerState> Players;

        /// <summary>
        /// Clones the game state.
        /// </summary>
        internal GameState Clone()
        {
            var zones = new ZoneState[Zones.Length];
            for (var i = 0; i < Zones.Length; i++)
            {
                Zones[i].Clone(ref zones[i]);
            }

            var players = new PooledList<PlayerState>(Players.Count, preallocate: true);
            for (var i = 0; i < players.Count; i++)
            {
                Players.Span[i].Clone(ref players.Span[i]);
            }

            TeamState team1 = default;
            TeamState team2 = default;
            Team1.Clone(ref team1);
            Team2.Clone(ref team2);

            return new GameState(team1, team2, zones, players);
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>The fully qualified type name.</returns>
        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine(Team1.ToString());
            builder.AppendLine(Team2.ToString());

            if (Zones != null)
            {
                foreach (var zoneState in Zones)
                {
                    builder.AppendLine(zoneState.ToString());
                }
            }

            if (Players != null)
            {
                builder.AppendLine("players: ");

                foreach (var playerState in Players)
                {
                    builder.AppendLine(playerState.ToString());
                }
            }

            return builder.ToString();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (Players != null)
            {
                for (int i = 0; i < Players.Count; i++)
                {
                    Players[i].Dispose();
                }
            }

            Players?.Dispose();
        }
    }
}