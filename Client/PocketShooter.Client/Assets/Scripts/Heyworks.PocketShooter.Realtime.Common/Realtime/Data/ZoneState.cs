using System;
using System.Runtime.CompilerServices;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.Realtime.State;

namespace Heyworks.PocketShooter.Realtime.Data
{
    public struct ZoneState : IEntity<byte>
    {
        // todo to entity id.

        /// <inheritdoc cref="IEntity{TId}"/>
        byte IEntity<byte>.Id => Id;

        /// <summary>
        /// Gets the zone identifier.
        /// </summary>
        public byte Id;

        /// <summary>
        /// Gets the team owner.
        /// </summary>
        /// <value>The team.</value>
        public TeamNo OwnerTeam;

        /// <summary>
        /// Gets the capture progress.
        /// </summary>
        public float Progress;

        /// <summary>
        /// Gets a value indicating whether this zone is captured.
        /// </summary>
        /// <value><c>true</c> if captured; otherwise, <c>false</c>.</value>
        public bool Captured;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoneState"/> struct.
        /// </summary>
        /// <param name="id">Identifier.</param>
        /// <param name="ownerTeam">Team.</param>
        /// <param name="progress">Progress.</param>
        /// <param name="captured">If set to <c>true</c> captured.</param>
        public ZoneState(byte id, TeamNo ownerTeam, byte progress, bool captured)
        {
            Id = id;
            OwnerTeam = ownerTeam;
            Progress = progress;
            Captured = captured;
        }

        /// <summary>
        /// Returns a description that represents the current <see cref="ZoneState"/>.
        /// </summary>
        /// <returns>A description that represents the current <see cref="ZoneState"/>.</returns>
        public override string ToString() =>
            $"id: {Id} : team: {OwnerTeam} : progress: {Progress} : captured: {Captured}";
    }

    /// <summary>
    /// Zone State Extensions.
    /// </summary>
    public static class ZoneStateExtensions
    {
        public static void Clone(this in ZoneState self, ref ZoneState to) => UnsafeClone.Clone(in self, ref to);
    }
}