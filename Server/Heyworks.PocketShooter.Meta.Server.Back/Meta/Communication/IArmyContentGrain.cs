using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Entities;
using Orleans;
using Orleans.Concurrency;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public interface IArmyContentGrain : IGrainWithGuidKey
    {
        Task AddTrooper(Immutable<TrooperIdentity> trooper);

        Task AddWeapon(Immutable<WeaponIdentity> weapon);

        Task AddHelmet(Immutable<HelmetIdentity> helmet);

        Task AddArmor(Immutable<ArmorIdentity> armor);

        Task AddOffensive(Immutable<OffensiveIdentity> offensive);

        Task AddSupport(Immutable<SupportIdentity> support);
    }
}
