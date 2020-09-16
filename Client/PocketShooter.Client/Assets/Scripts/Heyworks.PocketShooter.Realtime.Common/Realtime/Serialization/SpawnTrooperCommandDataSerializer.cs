using System.Runtime.CompilerServices;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.Realtime.Serialization;

namespace Heyworks.PocketShooter.Realtime.Serialization
{
    public class SpawnTrooperCommandDataSerializer : DataSerializer<SpawnTrooperCommandData>
    {
        public override SpawnTrooperCommandData Deserialize(byte[] data)
        {
            var inStream = new TypedBitInStream<IServiceCommandData>(data);

            return inStream.ReadOne<SpawnTrooperCommandData>();
        }

        public override byte[] Serialize(SpawnTrooperCommandData data)
        {
            var outStream = new TypedBitOutStream<IServiceCommandData>(Unsafe.SizeOf<SpawnTrooperCommandData>());
            outStream.WriteOne(data);

            return outStream.ToArray();
        }
    }
}