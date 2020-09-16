using System.Threading.Tasks;
using System;
using Heyworks.PocketShooter.Meta.Configuration.Data;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public interface IGameHubClient
    {
        ValueTask ConnectAsync();

        ValueTask<ResponseOption<GameConfig>> SynchronizeConfigAsync(Version configVersion);

        ValueTask<ResponseOption> ChangeNickname(string newNickname);

        ValueTask MakeMatchAsync(MatchRequest requestedMatch);

        ValueTask<ResponseOption<MatchResultsData>> GetMatchResultsAsync(Guid roomId);

        ValueTask<ResponseOption> MakePurchaseAsync(string receiptData);

        ValueTask BuyRosterProductAsync(string productId);

        ValueTask BuyShopProductAsync(string productId);

        ValueTask EquipWeaponAsync(TrooperClass tropperToEquip, WeaponName weaponToEquip);

        ValueTask EquipHelmetAsync(TrooperClass tropperToEquip, HelmetName helmetToEquip);

        ValueTask EquipArmorAsync(TrooperClass tropperToEquip, ArmorName armorToEquip);

        ValueTask DisconnectAsync();

        void Subscribe(IGameHubObserver observer);

        ValueTask LevelUpTrooperInstant(TrooperClass trooperClass);

        ValueTask LevelUpTrooperRegular(TrooperClass trooperClass);

        ValueTask GradeUpTrooperInstant(TrooperClass trooperClass);

        ValueTask LevelUpWeaponInstant(WeaponName name);

        ValueTask LevelUpWeaponRegular(WeaponName name);

        ValueTask GradeUpWeaponInstant(WeaponName name);

        ValueTask LevelUpHelmetInstant(HelmetName name);

        ValueTask LevelUpHelmetRegular(HelmetName name);

        ValueTask GradeUpHelmetInstant(HelmetName name);

        ValueTask LevelUpArmorInstant(ArmorName name);

        ValueTask LevelUpArmorRegular(ArmorName name);

        ValueTask GradeUpArmorInstant(ArmorName name);

        ValueTask SyncArmyItemProgress();

        ValueTask CompleteArmyItemProgress();

        ValueTask CollectRepeatingReward();

        ValueTask Cheat_UnlockContent();

        ValueTask Cheat_AddResources(int cash, int gold);

        ValueTask Cheat_LevelUpPlayer();
    }
}
