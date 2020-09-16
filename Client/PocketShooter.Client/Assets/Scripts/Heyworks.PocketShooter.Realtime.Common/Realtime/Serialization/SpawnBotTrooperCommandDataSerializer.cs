using System.Runtime.CompilerServices;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.Realtime.Serialization;

namespace Heyworks.PocketShooter.Realtime.Serialization
{
    public class SpawnBotTrooperCommandDataSerializer : DataSerializer<SpawnBotTrooperCommandData>
    {
        public override SpawnBotTrooperCommandData Deserialize(byte[] data)
        {
            var inStream = new TypedBitInStream<IServiceCommandData>(data);

            return inStream.ReadOne<SpawnBotTrooperCommandData>();
        }

        public override byte[] Serialize(SpawnBotTrooperCommandData data)
        {
            var outStream = new TypedBitOutStream<IServiceCommandData>(Unsafe.SizeOf<SpawnBotTrooperCommandData>());
            outStream.WriteOne(data);

            return outStream.ToArray();
        }
    }
}