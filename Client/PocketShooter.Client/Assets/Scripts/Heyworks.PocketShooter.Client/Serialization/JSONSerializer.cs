using System;
using System.Runtime.Serialization;
using Heyworks.PocketShooter.Realtime;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Heyworks.PocketShooter.Serialization
{
    /// <summary>
/// Represents a json data serializer.
/// </summary>
public class JSONSerializer : IDataSerializer
{
    private readonly JsonSerializerSettings serializerSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="JSONSerializer"/> class using default serialization settings.
    /// </summary>
    public JSONSerializer()
    {
        serializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            ContractResolver = new CanWritePropertiesOnlyResolver(),
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
        };

        serializerSettings.Converters.Add(new IsoDateTimeConverter { DateTimeFormat = Constants.DateTimeFormat });
    }

    /// <summary>
    /// Deserializes the JSON string to the specified .NET type.
    /// </summary>
    /// <param name="serializedData">The serialized JSON string data to deserialize.</param>
    /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
    /// <exception cref="SerializationException">Thrown when an error occurs during Json serialization or deserialization.</exception>
    /// <returns>The deserialized object from the string.</returns>
    public T Deserialize<T>(string serializedData)
    {
        return (T)Deserialize(serializedData, typeof(T));
    }

    /// <summary>
    /// Deserializes the JSON string to the specified .NET type.
    /// </summary>
    /// <param name="serializedData">The serialized JSON string data to deserialize.</param>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <exception cref="SerializationException">Thrown when an error occurs during Json serialization or deserialization.</exception>
    /// <returns>The deserialized object from the string.</returns>
    public object Deserialize(string serializedData, Type type)
    {
        try
        {
            return JsonConvert.DeserializeObject(serializedData, type, serializerSettings);
        }
        catch (JsonSerializationException ex)
        {
            SerializationLog.Log.LogError("An error has occurred while deserializing data. Error message: {0}, Type: {1}, Data: {2}", ex.Message, type, serializedData);
            throw new SerializationException("An error has occurred while deserializing data. See logs for more info", ex);
        }
        catch (JsonReaderException ex)
        {
            SerializationLog.Log.LogError("An error has occurred while deserializing data. Error message: {0}, Type: {1}, Data: {2}", ex.Message, type, serializedData);
            throw new SerializationException("An error has occurred while deserializing data. See logs for more info", ex);
        }
    }

    /// <summary>
    /// Serializes the specified object to a JSON string.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <param name="format">Specifies formatting options for the <see cref="JsonTextWriter"/></param>
    /// <exception cref="SerializationException">Thrown when an error occurs during Json serialization or deserialization.</exception>
    /// <returns>A JSON string representation of the object.</returns>
    public string Serialize(object obj, Formatting format)
    {
        try
        {
            return JsonConvert.SerializeObject(obj, format, serializerSettings);
        }
        catch (JsonSerializationException ex)
        {
            SerializationLog.Log.LogError("An error has occurred while serializing object. Error message: {0}, Type: {1}, Object: {2}", ex.Message, obj.GetType(), obj);
            throw new SerializationException("An error has occurred while serializing object. See logs for more info", ex);
        }
        catch (JsonWriterException ex)
        {
            SerializationLog.Log.LogError("An error has occurred while serializing object. Error message: {0}, Type: {1}, Object: {2}", ex.Message, obj.GetType(), obj);
            throw new SerializationException("An error has occurred while serializing object. See logs for more info", ex);
        }
    }

    /// <summary>
    /// Serializes the specified object to a JSON string.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <exception cref="SerializationException">Thrown when an error occurs during Json serialization or deserialization.</exception>
    /// <returns>A JSON string representation of the object.</returns>
    public string Serialize(object obj)
    {
        return Serialize(obj, Formatting.None);
    }

    /// <summary>
    /// Gets the serializer settings.
    /// </summary>
    public JsonSerializerSettings Settings => serializerSettings;
}

}