using System;
using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Serialization
{
    public readonly struct CommandHeader : IEquatable<CommandHeader>
    {
        public CommandHeader(EntityId id, SimulationDataCode code)
        {
            Id = id;
            Code = code;
        }

        public readonly EntityId Id;
        public readonly SimulationDataCode Code;

        public override int GetHashCode() => (Id, Code).GetHashCode();
        public override string ToString() => (Id, Code).ToString();
        public bool Equals(CommandHeader other) => Id == other.Id && Code == other.Code;
        public override bool Equals(object obj) => obj is CommandHeader other && Equals(other);
        public static implicit operator CommandHeader((EntityId id, SimulationDataCode code) value) => new CommandHeader(value.id, value.code);
    }
}
