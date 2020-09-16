using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.Realtime.Serialization;
using NetStack.Serialization;

namespace Heyworks.PocketShooter.Realtime.Serialization
{
    public class BotControlTakenDataSerializer : DataSerializer<BotControlTakenData>
    {
        public override BotControlTakenData Deserialize(byte[] data)
        {
            var inStream = new TypedBitInStream<BotControlTakenData>(data, new BitBufferOptions(u8SpanBitsLength: 16));

            var playerInfoSize = inStream.Stream.u8SpanLengthPeek();
            var playerInfoData = new byte[playerInfoSize];
            inStream.Stream.u8(playerInfoData);

            var teamNo = inStream.ReadOne<TeamNo>();

            return new BotControlTakenData(
                CompressedJson.DeCompressAndDeSerialize<PlayerInfo>(playerInfoData),
                teamNo,
                inStream.ReadOne<EntityId>());
        }

        public override byte[] Serialize(BotControlTakenData data)
        {
            var outStream = new TypedBitOutStream<BotControlTakenData>(ushort.MaxValue, new BitBufferOptions(u8SpanBitsLength: 16));
            outStream.Stream.u8(CompressedJson.SerializeAndCompress(data.BotInfo));
            outStream.WriteOne(data.TeamNo);
            outStream.WriteOne(data.TrooperId);
            return outStream.ToArray();
        }
    }
}