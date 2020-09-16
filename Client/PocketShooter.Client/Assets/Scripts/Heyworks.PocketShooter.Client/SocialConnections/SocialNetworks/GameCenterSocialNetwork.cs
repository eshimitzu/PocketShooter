using Heyworks.PocketShooter.Meta.Communication;
using UniRx.Async;

namespace Heyworks.PocketShooter.SocialConnections.SocialNetworks
{
    public class GameCenterSocialNetwork : ISocialNetwork
    {
        public SocialNetworkName Name => SocialNetworkName.GameCenter;

        public SocialNetworkUser Player => null;

        public bool IsConnected => false;

        public async UniTask Initialize()
        {
        }

        public async UniTask<ResponseOption<SocialNetworkAccessData>> LogIn()
        {
            return null;
        }

        public void LogOut()
        {
        }
    }
}