using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Data;
using Orleans;
using Orleans.Concurrency;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public interface IArmyGrain : IGrainWithGuidKey
    {
        Task Create();

        /// <summary>
        /// Returns false if failed to equip for any reason.
        /// </summary>
        Task<bool> EquipWeapon(TrooperClass trooperToEquip, WeaponName weaponToEquip);

        Task<bool> EquipHelmet(TrooperClass trooperToEquip, HelmetName helmetToEquip);

        Task<bool> EquipArmor(TrooperClass trooperToEquip, ArmorName armorToEquip);

        Task<bool> LevelUpTrooperInstant(TrooperClass trooperClass);

        Task<bool> LevelUpTrooperRegular(TrooperClass trooperClass);

        Task<bool> GradeUpTrooperInstant(TrooperClass trooperClass);

        Task<bool> LevelUpWeaponInstant(WeaponName weaponName);

        Task<bool> LevelUpWeaponRegular(WeaponName weaponName);

        Task<bool> GradeUpWeaponInstant(WeaponName weaponName);

        Task<bool> LevelUpHelmetInstant(HelmetName helmetName);

        Task<bool> LevelUpHelmetRegular(HelmetName helmetName);

        Task<bool> GradeUpHelmetInstant(HelmetName helmetName);

        Task<bool> LevelUpArmorInstant(ArmorName armorName);

        Task<bool> LevelUpArmorRegular(ArmorName armorName);

        Task<bool> GradeUpArmorInstant(ArmorName armorName);

        Task SyncItemProgress();

        Task<bool> CompleteItemProgress();

        [AlwaysInterleave]
        Task<ServerArmyState> GetState();
    }
}