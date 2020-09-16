using Heyworks.PocketShooter.Communication;
using Heyworks.PocketShooter.Meta.Communication;

namespace Heyworks.PocketShooter.Meta.Realtime
{
    // ISSUE: not real, should be fully async
    public interface IMatchMakingService
    {
         ServerAddress GetServer();
    }
}