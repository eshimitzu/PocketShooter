using System.Collections.Generic;
using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Entities
{
    /// <summary>
    /// Game match team.
    /// </summary>
    public class Team : ITeam
    {
        private readonly TeamInfo teamInfo;
        private readonly ISet<byte> zonesCaptured;

        private IRef<TeamState> teamStateRef;
        private int currentSpawnPoint;

        /// <summary>
        /// Gets team number.
        /// </summary>
        public TeamNo Id => teamStateRef.Value.Number;

        ref readonly TeamState ITeam.State => ref teamStateRef.Value;

        public ref TeamState State => ref teamStateRef.Value;

        public void ApplyState(IRef<TeamState> teamStateRef)
        {
            this.teamStateRef = teamStateRef;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Team"/> class.
        /// </summary>
        /// <param name="teamStateRef">The reference to team state.</param>
        /// <param name="teamInfo">The information about team.</param>
        public Team(IRef<TeamState> teamStateRef, TeamInfo teamInfo)
        {
            this.teamStateRef = teamStateRef;
            this.teamInfo = teamInfo;
            zonesCaptured = new HashSet<byte>();
        }

        /// <summary>
        /// Gets the amount of zones captured by the team.
        /// </summary>
        // TODO: v.shimkovich server only, consider inheritance
        public int ZonesCaptured => zonesCaptured.Count;

        /// <summary>
        /// Gets next spawn point for the player of the team.
        /// </summary>
        public byte NextSpawnPoint => (byte)(currentSpawnPoint++ % teamInfo.SpawnPoints.Length);

        /// <summary>
        /// Captures the zone.
        /// </summary>
        /// <param name="zoneId">Id of the zone to capture.</param>
        public void CaptureZone(byte zoneId)
        {
            zonesCaptured.Add(zoneId);
        }

        /// <summary>
        /// Releases the zone.
        /// </summary>
        /// <param name="zoneId">Id of the zone to release.</param>
        public void ReleaseZone(byte zoneId)
        {
            zonesCaptured.Remove(zoneId);
        }
    }
}