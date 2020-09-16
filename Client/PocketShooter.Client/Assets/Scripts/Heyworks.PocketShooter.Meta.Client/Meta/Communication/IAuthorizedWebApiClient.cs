using System.Threading.Tasks;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public interface IAuthorizedWebApiClient
    {
        void StartNewSession();

        ValueTask<ResponseOption<SocialConnectResponseData>> ConnectGooglePlay(SocialConnectRequest connectRequest);
    }
}