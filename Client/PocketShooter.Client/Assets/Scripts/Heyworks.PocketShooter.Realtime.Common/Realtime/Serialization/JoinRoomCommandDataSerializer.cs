using System.Runtime.CompilerServices;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.Realtime.Serialization;

namespace Heyworks.PocketShooter.Realtime.Serialization
{
    public class JoinRoomCommandDataSerializer : DataSerializer<JoinRoomCommandData>
    {
        public override JoinRoomCommandData Deserialize(byte[] data)
        {
            var inStream = new TypedBitInStream<IServiceCommandData>(data);

            return inStream.ReadOne<JoinRoomCommandData>();
        }

        public override byte[] Serialize(JoinRoomCommandData data)
        {
            var outStream = new TypedBitOutStream<IServiceCommandData>(Unsafe.SizeOf<JoinRoomCommandData>());
            outStream.WriteOne(data.PlayerId);

            return outStream.ToArray();
        }
    }
}