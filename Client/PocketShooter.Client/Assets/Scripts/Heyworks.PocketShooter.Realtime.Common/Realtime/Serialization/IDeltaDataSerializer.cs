using System.Collections.Generic;

namespace Heyworks.Realtime.Serialization
{
    public interface IDeltaDataSerializer<T>
        where T : struct
    {
        byte[] Serialize(in T? baseline, in T data);

        T? Deserialize(byte[] data);
    }
}