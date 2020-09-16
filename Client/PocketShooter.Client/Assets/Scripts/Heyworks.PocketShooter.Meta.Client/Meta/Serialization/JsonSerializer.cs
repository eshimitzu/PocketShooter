using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Heyworks.PocketShooter.Meta.Serialization
{
    public class JsonSerializer : IDataSerializer
    {
        private readonly JsonSerializerSettings serializerSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="JSONSerializer"/> class using default serialization settings.
        /// </summary>
        public JsonSerializer(JsonSerializerSettings serializerSettings)
        {
            this.serializerSettings = serializerSettings;
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
                string message = $"An error has occurred while deserializing data. Error message: {ex.Message}, Type: {type}, Data: {serializedData}";
                GameLog.Log.LogError(ex, message);

                throw new SerializationException(message, ex);
            }
            catch (JsonReaderException ex)
            {
                string message = $"An error has occurred while deserializing data. Error message: {ex.Message}, Type: {type}, Data: {serializedData}";
                GameLog.Log.LogError(ex, message);

                throw new SerializationException(message, ex);
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
                string message = $"An error has occurred while serializing object. Error message: {ex.Message}, Type: {obj.GetType()}, Object: {obj}";
                GameLog.Log.LogError(ex, message);

                throw new SerializationException(message, ex);
            }
            catch (JsonWriterException ex)
            {
                string message = $"An error has occurred while serializing object. Error message: {ex.Message}, Type: {obj.GetType()}, Object: {obj}";
                GameLog.Log.LogError(ex, message);

                throw new SerializationException(message, ex);
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
    }
}
