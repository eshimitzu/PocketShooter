using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Entities;
using Orleans;
using Orleans.Concurrency;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public interface IPlayerBalanceGrain : IGrainWithGuidKey
    {
        Task PayPrice(Immutable<Price> price);

        Task RegisterPurchase(Immutable<InAppPurchase> purchase);
    }
}
