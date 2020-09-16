using Heyworks.PocketShooter.Serialization;
using Newtonsoft.Json;

/// <summary>
/// Generic class for all serializable configurations.
/// </summary>
/// <typeparam name="T">The type of configuration.</typeparam>
public class SerializableConfiguration<T>
{
    /// <summary>
    /// Deserializes a configuration.
    /// </summary>
    /// <param name="data">The configuration data to deserialize.</param>
    public static T Deserialize(string data)
    {
        var serializer = new JSONSerializer();
        return serializer.Deserialize<T>(data);
    }

    /// <summary>
    /// Serializes a configuration.
    /// </summary>
    public string Serialize()
    {
        var serializer = new JSONSerializer();
        return serializer.Serialize(this);
    }

    /// <summary>
    /// Serializes a configuration.
    /// </summary>
    public string SerializeWithIndentedFormatting()
    {
        var serializer = new JSONSerializer();
        return serializer.Serialize(this, Formatting.Indented);
    }

    /// <summary>
    /// Clone the object using serialization.
    /// </summary>
    public T Clone()
    {
        var data = Serialize();
        return Deserialize(data);
    }
}
