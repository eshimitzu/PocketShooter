using System.Runtime.CompilerServices;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.Realtime.Serialization;

namespace Heyworks.PocketShooter.Realtime.Serialization
{
    public sealed class GameEndedDataSerializer : DataSerializer<GameEndedData>
    {
        public override GameEndedData Deserialize(byte[] data)
        {
            var inStream = new TypedBitInStream<GameEndedData>(data);

            return inStream.ReadOne<GameEndedData>();
        }

        public override byte[] Serialize(GameEndedData data)
        {
            var outStream = new TypedBitOutStream<GameEndedData>(Unsafe.SizeOf<GameEndedData>());
            outStream.WriteOne(data);

            return outStream.ToArray();
        }
    }
}
