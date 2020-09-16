using Heyworks.PocketShooter.Meta.Communication;
using System.Net.Http;
using System.Threading.Tasks;

namespace Heyworks.PocketShooter.Meta.Serialization
{
    public static class HttpResponseMessageExtensions
    {
        public static ValueTask<ResponseOption<TOkData>> GetResponseOptionAsync<TOkData>(this HttpResponseMessage responseMessage, IDataSerializer dataSerializer) =>
            GetResponseOptionAsync<TOkData, ResponseError>(responseMessage, dataSerializer);

        public static async ValueTask<ResponseOption<TOkData>> GetResponseOptionAsync<TOkData, TError>(this HttpResponseMessage responseMessage, IDataSerializer dataSerializer)
            where TError : ResponseError
        {
            if (responseMessage.IsSuccessStatusCode)
            {
                return ResponseOk.CreateOption(await GetResponseOkContentAsync<TOkData>(responseMessage, dataSerializer));
            }
            else
            {
                return ResponseError.CreateOption<TOkData>(await GetResponseErrorAsync<TError>(responseMessage, dataSerializer));
            }
        }

        private static async ValueTask<ResponseOk<T>> GetResponseOkContentAsync<T>(HttpResponseMessage responseMessage, IDataSerializer dataSerializer)
        {
            return await responseMessage.Content.ReadAsAsync<ResponseOk<T>>(dataSerializer);
        }

        private static async ValueTask<TError> GetResponseErrorAsync<TError>(HttpResponseMessage responseMessage, IDataSerializer dataSerializer)
            where TError : ResponseError
        {
            return await responseMessage.Content.ReadAsAsync<TError>(dataSerializer);
        }
    }
}
