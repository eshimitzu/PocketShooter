namespace Heyworks.PocketShooter.Realtime.Serialization
{

    public interface IDataSerializer<T>
    {
        byte[] Serialize(T data);

        T Deserialize(byte[] data);
    }

    public interface IDataSerializer
    {
        // NOTE: each return (if struct) creates GC box garbage
        object Deserialize(byte[] data);

        // NOTE: perfromance - should receive buffer to write into, not return new allocated one
        byte[] Serialize(object data);
    }

    public abstract class DataSerializer<T> : IDataSerializer<T>, IDataSerializer
    {
        public abstract T Deserialize(byte[] data);

        public abstract byte[] Serialize(T data);

        object IDataSerializer.Deserialize(byte[] data) => Deserialize(data);

        byte[] IDataSerializer.Serialize(object data) => Serialize((T)data);
    }
}