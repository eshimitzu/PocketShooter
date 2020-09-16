namespace Heyworks.PocketShooter.Serialization
{
    public interface IDataSerializer
    {
        string Serialize(object obj);

        T Deserialize<T>(string str);
    }
}