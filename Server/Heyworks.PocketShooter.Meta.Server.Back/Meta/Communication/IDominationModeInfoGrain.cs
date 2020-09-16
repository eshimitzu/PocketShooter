using System.Threading.Tasks;
using Heyworks.PocketShooter.Realtime.Data;
using Orleans;
using Orleans.Concurrency;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public interface IDominationModeInfoGrain : IGrainWithGuidKey
    {
        Task<Immutable<DominationModeInfo>> GetDominationModeInfo(MapNames map);
        Task<int> GetModeLimits();
    }
}
