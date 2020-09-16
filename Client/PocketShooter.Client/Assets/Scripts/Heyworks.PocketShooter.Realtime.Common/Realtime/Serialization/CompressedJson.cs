using System.IO;
using System.IO.Compression;
using Heyworks.PocketShooter.Meta.Serialization;
using Newtonsoft.Json;

namespace Heyworks.PocketShooter.Realtime.Serialization
{
    // Is not optimized at all - just very seldom couple times raw compressed data into client from meta.
    // https://stackoverflow.com/questions/32871013/jsonserializer-failing-to-write-to-a-gzipstream
    // TODO: use span/ref/memory pool/array pool/stream pool for perf in loop

    public static class CompressedJson
    {
        internal static byte[] SerializeAndCompress<T>(T objectToSerialize)
        {
            using (var memStream = new MemoryStream())
            {
                using (var zipStream = new GZipStream(memStream, CompressionMode.Compress, true))
                using (var streamWriter = new StreamWriter(zipStream))
                using (var jsonWriter = new JsonTextWriter(streamWriter))
                {
                    // NOTE: Could use BSON or MSGPACK here
                    var jsonSerializer = JsonSerializer.Create(new DefaultSerializerSettings());

                    jsonSerializer.Serialize(jsonWriter, objectToSerialize);
                }

                return memStream.ToArray();
            }
        }

        internal static T DeCompressAndDeSerialize<T>(byte[] data)
        {
            using (var memStream = new MemoryStream(data))
            {
                using (var zipStream = new GZipStream(memStream, CompressionMode.Decompress, true))
                using (var streamReader = new StreamReader(zipStream))
                using (var jsonReader = new JsonTextReader(streamReader))
                {
                    // NOTE: Could use BSON or MSGPACK here
                    var jsonSerializer = JsonSerializer.Create(new DefaultSerializerSettings());

                    return jsonSerializer.Deserialize<T>(jsonReader);
                }
            }
        }
    }
}
