using System;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.Realtime.Serialization;
using NetStack.Serialization;

namespace Heyworks.PocketShooter.Realtime.Serialization
{
    public class GameJoinedDataSerializer : DataSerializer<GameJoinedData>
    {
        public override GameJoinedData Deserialize(byte[] data)
        {
            var inStream = new TypedBitInStream<GameJoinedData>(data, new BitBufferOptions(u8SpanBitsLength: 16));

            var modeInfoSize = inStream.Stream.u8SpanLengthPeek();
            var modeInfoData = new byte[modeInfoSize];
            inStream.Stream.u8(modeInfoData);

            var roomId = inStream.ReadOne<RoomId>();
            var teamNo = inStream.ReadOne<TeamNo>();

            var playerInfoSize = inStream.Stream.u8SpanLengthPeek();
            var playerInfoData = new byte[playerInfoSize];
            inStream.Stream.u8(playerInfoData);

            return new GameJoinedData(
                CompressedJson.DeCompressAndDeSerialize<DominationModeInfo>(modeInfoData),
                roomId,
                teamNo,
                CompressedJson.DeCompressAndDeSerialize<PlayerInfo>(playerInfoData),
                inStream.ReadOne<EntityId>(),
                inStream.ReadInt(),
                inStream.ReadInt());
        }

        public override byte[] Serialize(GameJoinedData data)
        {
            // need to be sure whole configuration gots into it
            var outStream = new TypedBitOutStream<GameJoinedData>(ushort.MaxValue, new BitBufferOptions(u8SpanBitsLength: 16));

            var modeInfo = CompressedJson.SerializeAndCompress(data.ModeInfo);
            outStream.Stream.u8(modeInfo);

            outStream.WriteOne(data.RoomId);
            outStream.WriteOne(data.TeamNo);

            var playerInfo = CompressedJson.SerializeAndCompress(data.PlayerInfo);
            outStream.Stream.u8(playerInfo);

            outStream.WriteOne(data.TrooperId);
            outStream.WriteInt(data.Tick);
            outStream.WriteInt(data.TimeStamp);
            return outStream.ToArray();
        }
    }
}