using System.Net.Http;
using System.Threading.Tasks;

namespace Heyworks.PocketShooter.Meta.Serialization
{
    /// <summary>
    /// Extension methods for <see cref="HttpContent"/> class.
    /// </summary>
    public static class HttpContentExtensions
    {
        /// <summary>
        /// Returns a <see cref="Task"/> that will yield an object of the specified
        /// type <typeparamref name="T"/> from the <paramref name="content"/> instance.
        /// </summary>
        /// <typeparam name="T">The type of the object to read.</typeparam>
        /// <param name="content">The <see cref="HttpContent"/> instance from which to read.</param>
        /// <param name="dataSerializer">The data serializer.</param>
        public static async Task<T> ReadAsAsync<T>(this HttpContent content, IDataSerializer dataSerializer)
        {
            var stringContent = await content.ReadAsStringAsync();

            return dataSerializer.Deserialize<T>(stringContent);
        }
    }
}
