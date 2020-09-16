using Heyworks.Realtime.Serialization;

namespace Heyworks.PocketShooter.Realtime.Data
{
    /// <summary>
    /// EntityRef.
    /// </summary>
    public readonly struct EntityRef
    {
        /// <summary>
        /// Entity type.
        /// </summary>
        [Limit(typeof(EntityType))]
        public readonly EntityType EntityType;

        /// <summary>
        /// Entity Id.
        /// </summary>
        public readonly EntityId EntityId;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityRef"/> struct.
        /// </summary>
        /// <param name="entityType">entityType.</param>
        /// <param name="entityId">entityId.</param>
        public EntityRef(EntityType entityType, EntityId entityId)
        {
            this.EntityType = entityType;
            this.EntityId = entityId;
        }

        public override string ToString() => $"{nameof(EntityRef)}{(EntityType, EntityId)}";
    }
}