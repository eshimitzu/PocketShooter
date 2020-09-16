using System.Net.Http;
using System.Text;

namespace Heyworks.PocketShooter.Meta.Serialization
{
    public static class DataSerializerExtensions
    {
        public static HttpContent CreateRequestContent(this IDataSerializer dataSerializer, object requestEntity)
        {
            return
                new StringContent(
                    dataSerializer.Serialize(requestEntity),
                    Encoding.UTF8,
                    "application/json");
        }
    }
}
