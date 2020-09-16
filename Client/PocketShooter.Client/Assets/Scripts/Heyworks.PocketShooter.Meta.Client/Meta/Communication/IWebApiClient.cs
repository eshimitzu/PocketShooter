using System.Net;
using System.Threading.Tasks;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public interface IWebApiClient
    {
        IPEndPoint Endpoint { get; }

        ValueTask<ResponseOption<RegisterResponseData>> RegisterDevice(RegisterRequest registerRequest);

        ValueTask<ResponseOption<LoginResponseData>> LoginDevice(LoginRequest loginRequest);

        ValueTask<ResponseOption<SocialLoginResponseData>> LoginGooglePlay(GoogleLoginRequest loginRequest);

        ValueTask<ResponseOption<SocialLoginResponseData>> LoginGameCenter();
    }
}
