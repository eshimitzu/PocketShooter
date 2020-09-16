using System.Linq;
using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Data;
using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.Meta.Runnables;
using Orleans;
using Orleans.Concurrency;
using Orleans.Providers;

namespace Heyworks.PocketShooter.Meta.Communication
{
    [StorageProvider(ProviderName = Server.Constants.GrainStorageProviderName)]
    public class ArmyGrain : Grain<ServerArmyState>,
        IArmyGrain,
        IArmyContentGrain,
        IConsumablesPublisherGrain,
        IArmyRunnablesGrain
    {
        private readonly IGameFactory gameFactory;

        public ArmyGrain(IGameFactory gameFactory)
        {
            this.gameFactory = gameFactory;
        }

        public async Task Create()
        {
            State = new ServerArmyState(this.GetPrimaryKey());

            await WriteStateAsync();
        }

        public Task<ServerArmyState> GetState() => Task.FromResult(State);

        public async Task<bool> EquipWeapon(TrooperClass trooperToEquip, WeaponName weaponToEquip)
        {
            var army = (await gameFactory.Create(this.GetPrimaryKey())).Army;

            if (!army.CanEquipWeapon(trooperToEquip, weaponToEquip))
            {
                return false;
            }

            army.EquipWeapon(trooperToEquip, weaponToEquip);

            await SaveArmyState(army);

            return true;
        }

        public async Task<bool> EquipHelmet(TrooperClass trooperToEquip, HelmetName helmetToEquip)
        {
            var army = (await gameFactory.Create(this.GetPrimaryKey())).Army;

            if (!army.CanEquipHelmet(trooperToEquip, helmetToEquip))
            {
                return false;
            }

            army.EquipHelmet(trooperToEquip, helmetToEquip);

            await SaveArmyState(army);

            return true;
        }

        public async Task<bool> EquipArmor(TrooperClass trooperToEquip, ArmorName armorToEquip)
        {
            var army = (await gameFactory.Create(this.GetPrimaryKey())).Army;

            if (!army.CanEquipArmor(trooperToEquip, armorToEquip))
            {
                return false;
            }

            army.EquipArmor(trooperToEquip, armorToEquip);

            await SaveArmyState(army);

            return true;
        }

        public async Task<bool> LevelUpTrooperInstant(TrooperClass trooperClass)
        {
            var game = await gameFactory.Create(this.GetPrimaryKey());
            if (!game.CanLevelUpTrooperInstant(trooperClass))
            {
                return false;
            }

            var price = game.Shop.GetTrooperInstantLevelUpPrice(trooperClass);
            await LevelUpTrooper(game.Army, trooperClass, price, true);

            return true;
        }

        public async Task<bool> LevelUpTrooperRegular(TrooperClass trooperClass)
        {
            var game = await gameFactory.Create(this.GetPrimaryKey());
            if (!game.CanLevelUpTrooperRegular(trooperClass))
            {
                return false;
            }

            var price = game.Shop.GetTrooperRegularLevelUpPrice(trooperClass);
            await LevelUpTrooper(game.Army, trooperClass, price, false);

            return true;
        }

        public async Task<bool> GradeUpTrooperInstant(TrooperClass trooperClass)
        {
            var game = await gameFactory.Create(this.GetPrimaryKey());
            if (!game.CanGardeUpTrooperInstant(trooperClass))
            {
                return false;
            }

            var price = game.Shop.GetTrooperInstantGradeUpPrice(trooperClass);
            await GradeUpTrooper(game.Army, trooperClass, price);

            return true;
        }

        public async Task<bool> LevelUpWeaponInstant(WeaponName weaponName)
        {
            var game = await gameFactory.Create(this.GetPrimaryKey());
            if (!game.CanLevelUpWeaponInstant(weaponName))
            {
                return false;
            }

            var price = game.Shop.GetWeaponInstantLevelUpPrice(weaponName);
            await LevelUpWeapon(game.Army, weaponName, price, true);

            return true;
        }

        public async Task<bool> LevelUpWeaponRegular(WeaponName weaponName)
        {
            var game = await gameFactory.Create(this.GetPrimaryKey());
            if (!game.CanLevelUpWeaponRegular(weaponName))
            {
                return false;
            }

            var price = game.Shop.GetWeaponRegularLevelUpPrice(weaponName);
            await LevelUpWeapon(game.Army, weaponName, price, false);

            return true;
        }

        public async Task<bool> GradeUpWeaponInstant(WeaponName weaponName)
        {
            var game = await gameFactory.Create(this.GetPrimaryKey());
            if (!game.CanGardeUpWeaponInstant(weaponName))
            {
                return false;
            }

            var price = game.Shop.GetWeaponInstantGradeUpPrice(weaponName);
            await GradeUpWeapon(game.Army, weaponName, price);

            return true;
        }

        public async Task<bool> LevelUpHelmetInstant(HelmetName helmetName)
        {
            var game = await gameFactory.Create(this.GetPrimaryKey());
            if (!game.CanLevelUpHelmetInstant(helmetName))
            {
                return false;
            }

            var price = game.Shop.GetHelmetInstantLevelUpPrice(helmetName);
            await LevelUpHelmet(game.Army, helmetName, price, true);

            return true;
        }

        public async Task<bool> LevelUpHelmetRegular(HelmetName helmetName)
        {
            var game = await gameFactory.Create(this.GetPrimaryKey());
            if (!game.CanLevelUpHelmetRegular(helmetName))
            {
                return false;
            }

            var price = game.Shop.GetHelmetRegularLevelUpPrice(helmetName);
            await LevelUpHelmet(game.Army, helmetName, price, false);

            return true;
        }

        public async Task<bool> GradeUpHelmetInstant(HelmetName helmetName)
        {
            var game = await gameFactory.Create(this.GetPrimaryKey());
            if (!game.CanGardeUpHelmetInstant(helmetName))
            {
                return false;
            }

            var price = game.Shop.GetHelmetInstantGradeUpPrice(helmetName);
            await GradeUpHelmet(game.Army, helmetName, price);

            return true;
        }

        public async Task<bool> LevelUpArmorInstant(ArmorName armorName)
        {
            var game = await gameFactory.Create(this.GetPrimaryKey());
            if (!game.CanLevelUpArmorInstant(armorName))
            {
                return false;
            }

            var price = game.Shop.GetArmorInstantLevelUpPrice(armorName);
            await LevelUpArmor(game.Army, armorName, price, true);

            return true;
        }

        public async Task<bool> LevelUpArmorRegular(ArmorName armorName)
        {
            var game = await gameFactory.Create(this.GetPrimaryKey());
            if (!game.CanLevelUpArmorRegular(armorName))
            {
                return false;
            }

            var price = game.Shop.GetArmorRegularLevelUpPrice(armorName);
            await LevelUpArmor(game.Army, armorName, price, false);

            return true;
        }

        public async Task<bool> GradeUpArmorInstant(ArmorName armorName)
        {
            var game = await gameFactory.Create(this.GetPrimaryKey());
            if (!game.CanGardeUpArmorInstant(armorName))
            {
                return false;
            }

            var price = game.Shop.GetArmorInstantGradeUpPrice(armorName);
            await GradeUpArmor(game.Army, armorName, price);

            return true;
        }

        public async Task SyncItemProgress()
        {
            var army = (await gameFactory.Create(this.GetPrimaryKey())).Army;

            IRunnable itemProgress = army.GetItemProgress();
            if (itemProgress != null && itemProgress.IsFinished)
            {
                itemProgress.Finish();
                await SaveArmyState(army);
            }
        }

        public async Task<bool> CompleteItemProgress()
        {
            var game = await gameFactory.Create(this.GetPrimaryKey());
            if (!game.CanCompleteArmyItemProgress())
            {
                return false;
            }

            var itemProgress = game.Army.GetItemProgress();
            itemProgress.Complete();

            var playerBalanceGrain = GrainFactory.GetGrain<IPlayerBalanceGrain>(this.GetPrimaryKey());
            await playerBalanceGrain.PayPrice(itemProgress.CompletePrice.AsImmutable());

            await SaveArmyState(game.Army);

            return true;
        }

        #region [IArmyRunnablesGrain Implementation]

        async Task IArmyRunnablesGrain.CheckRunnables()
        {
            var game = await gameFactory.Create(this.GetPrimaryKey());
            IHasRunnables hasRunnables = game.Army;

            var finishedRunnables =
                hasRunnables
                .GetRunnables()
                .Where(item => item.IsFinished)
                .OrderBy(item => item.RemainingTime)
                .ToList();

            foreach (var runnable in finishedRunnables)
            {
                runnable.Finish();
            }

            await SaveArmyState(game.Army);
        }

        #endregion [IArmyRunnablesGrain Implementation]

        #region [IArmyContentGrain Implementation]

        async Task IArmyContentGrain.AddTrooper(Immutable<TrooperIdentity> trooper)
        {
            var army = (await gameFactory.Create(this.GetPrimaryKey())).Army;

            army.AddTrooper(trooper.Value);

            await SaveArmyState(army);
        }

        async Task IArmyContentGrain.AddWeapon(Immutable<WeaponIdentity> weapon)
        {
            var army = (await gameFactory.Create(this.GetPrimaryKey())).Army;

            army.AddWeapon(weapon.Value);

            await SaveArmyState(army);
        }

        async Task IArmyContentGrain.AddHelmet(Immutable<HelmetIdentity> helmet)
        {
            var army = (await gameFactory.Create(this.GetPrimaryKey())).Army;

            army.AddHelmet(helmet.Value);

            await SaveArmyState(army);
        }

        async Task IArmyContentGrain.AddArmor(Immutable<ArmorIdentity> armor)
        {
            ServerArmy army = (await gameFactory.Create(this.GetPrimaryKey())).Army;

            army.AddArmor(armor.Value);

            await SaveArmyState(army);
        }

        async Task IArmyContentGrain.AddOffensive(Immutable<OffensiveIdentity> offensive)
        {
            var army = (await gameFactory.Create(this.GetPrimaryKey())).Army;

            army.AddOffensive(offensive.Value);

            await SaveArmyState(army);
        }

        async Task IArmyContentGrain.AddSupport(Immutable<SupportIdentity> support)
        {
            var army = (await gameFactory.Create(this.GetPrimaryKey())).Army;

            army.AddSupport(support.Value);

            await SaveArmyState(army);
        }

        #endregion [IArmyContentGrain Implementation]

        #region [IConsumablesPublisherGrain Implementation]

        async Task IConsumablesPublisherGrain.UpdateConsumables(int usedOffensives, int usedSupports)
        {
            var army = (await gameFactory.Create(this.GetPrimaryKey())).Army;

            army.RemoveSelectedOffensive(usedOffensives);
            army.RemoveSelectedSupport(usedSupports);

            await SaveArmyState(army);
        }

        #endregion [IConsumablesPublisherGrain Implementation]

        private async Task LevelUpTrooper(ServerArmy army, TrooperClass trooperClass, Price price, bool isInstant)
        {
            var playerBalanceGrain = GrainFactory.GetGrain<IPlayerBalanceGrain>(this.GetPrimaryKey());
            await playerBalanceGrain.PayPrice(price.AsImmutable());

            if (isInstant)
            {
                army.LevelUpTrooper(trooperClass);
            }
            else
            {
                var trooper = army.GetTrooper(trooperClass);
                army.StartItemProgress(trooper.Id);
            }

            await SaveArmyState(army);
        }

        private async Task GradeUpTrooper(ServerArmy army, TrooperClass trooperClass, Price price)
        {
            var playerBalanceGrain = GrainFactory.GetGrain<IPlayerBalanceGrain>(this.GetPrimaryKey());
            await playerBalanceGrain.PayPrice(price.AsImmutable());

            army.GradeUpTrooper(trooperClass);

            await SaveArmyState(army);
        }

        private async Task LevelUpWeapon(ServerArmy army, WeaponName weaponName, Price price, bool isInstant)
        {
            var playerBalanceGrain = GrainFactory.GetGrain<IPlayerBalanceGrain>(this.GetPrimaryKey());
            await playerBalanceGrain.PayPrice(price.AsImmutable());

            if (isInstant)
            {
                army.LevelUpWeapon(weaponName);
            }
            else
            {
                var weapon = army.GetWeapon(weaponName);
                army.StartItemProgress(weapon.Id);
            }

            await SaveArmyState(army);
        }

        private async Task GradeUpWeapon(ServerArmy army, WeaponName weaponName, Price price)
        {
            var playerBalanceGrain = GrainFactory.GetGrain<IPlayerBalanceGrain>(this.GetPrimaryKey());
            await playerBalanceGrain.PayPrice(price.AsImmutable());

            army.GradeUpWeapon(weaponName);

            await SaveArmyState(army);
        }

        private async Task LevelUpHelmet(ServerArmy army, HelmetName helmetName, Price price, bool isInstant)
        {
            var playerBalanceGrain = GrainFactory.GetGrain<IPlayerBalanceGrain>(this.GetPrimaryKey());
            await playerBalanceGrain.PayPrice(price.AsImmutable());

            if (isInstant)
            {
                army.LevelUpHelmet(helmetName);
            }
            else
            {
                var helmet = army.GetHelmet(helmetName);
                army.StartItemProgress(helmet.Id);
            }

            await SaveArmyState(army);
        }

        private async Task GradeUpHelmet(ServerArmy army, HelmetName helmetName, Price price)
        {
            var playerBalanceGrain = GrainFactory.GetGrain<IPlayerBalanceGrain>(this.GetPrimaryKey());
            await playerBalanceGrain.PayPrice(price.AsImmutable());

            army.GradeUpHelmet(helmetName);

            await SaveArmyState(army);
        }

        private async Task LevelUpArmor(ServerArmy army, ArmorName armorName, Price price, bool isInstant)
        {
            var playerBalanceGrain = GrainFactory.GetGrain<IPlayerBalanceGrain>(this.GetPrimaryKey());
            await playerBalanceGrain.PayPrice(price.AsImmutable());

            if (isInstant)
            {
                army.LevelUpArmor(armorName);
            }
            else
            {
                var armor = army.GetArmor(armorName);
                army.StartItemProgress(armor.Id);
            }

            await SaveArmyState(army);
        }

        private async Task GradeUpArmor(ServerArmy army, ArmorName armorName, Price price)
        {
            var playerBalanceGrain = GrainFactory.GetGrain<IPlayerBalanceGrain>(this.GetPrimaryKey());
            await playerBalanceGrain.PayPrice(price.AsImmutable());

            army.GradeUpArmor(armorName);

            await SaveArmyState(army);
        }

        private async Task SaveArmyState(ServerArmy army)
        {
            State = army.GetState();
            await WriteStateAsync();
        }
    }
}
