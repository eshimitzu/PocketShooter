using System;
using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Configuration.Data;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public interface IGameHub
    {
        Task<ResponseOption<GameConfig>> SynchronizeConfig(Version configVersion);

        Task<ResponseOption> ChangeNickname(string newNickname);

        Task MakeMatch(MatchRequest requestedMatch);

        Task<ResponseOption<MatchResultsData>> GetMatchResults(Guid roomId);

        Task<ResponseOption> MakePurchase(string purchaseReceipt);

        Task BuyRosterProduct(string productId);

        Task BuyShopProduct(string productId);

        Task EquipWeapon(TrooperClass trooperToEquip, WeaponName weaponToEquip);

        Task EquipHelmet(TrooperClass trooperToEquip, HelmetName helmetToEquip);

        Task EquipArmor(TrooperClass trooperToEquip, ArmorName armorToEquip);

        Task LevelUpTrooperInstant(TrooperClass trooperClass);

        Task LevelUpTrooperRegular(TrooperClass trooperClass);

        Task GradeUpTrooperInstant(TrooperClass trooperClass);

        Task LevelUpWeaponInstant(WeaponName name);

        Task LevelUpWeaponRegular(WeaponName name);

        Task GradeUpWeaponInstant(WeaponName name);

        Task LevelUpHelmetInstant(HelmetName name);

        Task LevelUpHelmetRegular(HelmetName name);

        Task GradeUpHelmetInstant(HelmetName name);

        Task LevelUpArmorInstant(ArmorName name);

        Task LevelUpArmorRegular(ArmorName name);

        Task GradeUpArmorInstant(ArmorName name);

        Task SyncArmyItemProgress();

        Task CompleteArmyItemProgress();

        Task CollectRepeatingReward();

        Task Cheat_UnlockContent();

        Task Cheat_LevelUpPlayer();

        Task Cheat_AddResources(int cash, int gold);
    }
}