using System;
using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime.Serialization
{
    public sealed class EmptyServiceCommandDataSerializer : DataSerializer<IServiceCommandData>
    {
        private readonly byte[] emptyData = new byte[0];

        public override IServiceCommandData Deserialize(byte[] data) => throw new NotSupportedException("Empty data can't be deserialized.");

        public override byte[] Serialize(IServiceCommandData data) => emptyData;
    }
}
