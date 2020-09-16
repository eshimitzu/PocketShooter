using System;
using System.Threading.Tasks;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public interface IAccessTokenProvider
    {
        Task<string> GetAccessToken();
    }
}
