using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Orleans;
using Orleans.Providers.MongoDB.StorageProviders;
using Orleans.Runtime;
using Orleans.Serialization;

namespace Heyworks.PocketShooter.Meta.Serialization
{
    internal class GrainStateSerializer : IGrainStateSerializer
    {
        private readonly JsonSerializer serializer;

        public GrainStateSerializer(ITypeResolver typeResolver, IGrainFactory grainFactory)
        {
            var serializerSettings = OrleansJsonSerializer.GetDefaultSerializerSettings(typeResolver, grainFactory);
            DefaultSerializerSettings.UpdateWithThisSettings(serializerSettings);

            this.serializer = JsonSerializer.Create(serializerSettings);
        }

        public void Deserialize(IGrainState grainState, JObject entityData)
        {
            grainState.State = serializer.Deserialize(new JTokenReader(entityData), grainState.State.GetType());
        }

        public JObject Serialize(IGrainState grainState) => JObject.FromObject(grainState.State, serializer);
    }
}
