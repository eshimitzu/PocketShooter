using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Meta.Data;
using Heyworks.PocketShooter.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public class GameHubClient : MetaHubClient, IGameHubClient
    {
        private readonly SynchronizationContext synchronizationContext;

        public GameHubClient(
            IPEndPoint serverAddress,
            string clientVersion,
            IAccessTokenProvider accessTokenProvider,
            SynchronizationContext synchronizationContext,
            ILoggerFactory loggerFactory,
            IConnectionListener connectionListener)
            : base(serverAddress, nameof(IGameHub), clientVersion, accessTokenProvider, loggerFactory, connectionListener)
        {
            this.synchronizationContext = synchronizationContext;
        }

        public void Subscribe(IGameHubObserver observer)
        {
            Connection.OnWithContext<MatchMakingResultData>(nameof(IGameHubObserver.MatchMaked), async x => await observer.MatchMaked(x), synchronizationContext);
            Connection.OnWithContext<ClientGameState>(nameof(IGameHubObserver.ReceiveGameState), async x => await observer.ReceiveGameState(x), synchronizationContext);
        }

        public ValueTask<ResponseOption<GameConfig>> SynchronizeConfigAsync(Version configVersion) =>
            InvokeAsync<GameConfig>(nameof(IGameHub.SynchronizeConfig), configVersion);

        public ValueTask<ResponseOption> ChangeNickname(string newNickname) =>
            InvokeAsync(nameof(IGameHub.ChangeNickname), newNickname);

        public ValueTask MakeMatchAsync(MatchRequest requestedMatch) =>
            SendAsync(nameof(IGameHub.MakeMatch), requestedMatch);

        public ValueTask<ResponseOption<MatchResultsData>> GetMatchResultsAsync(Guid roomId) =>
            InvokeAsync<MatchResultsData>(nameof(IGameHub.GetMatchResults), roomId);

        public ValueTask<ResponseOption> MakePurchaseAsync(string purchaseReceipt) =>
            InvokeAsync(nameof(IGameHub.MakePurchase), purchaseReceipt);

        public ValueTask BuyRosterProductAsync(string productId) =>
            SendAsync(nameof(IGameHub.BuyRosterProduct), productId);

        public ValueTask BuyShopProductAsync(string productId) =>
            SendAsync(nameof(IGameHub.BuyShopProduct), productId);

        public ValueTask EquipWeaponAsync(TrooperClass tropperToEquip, WeaponName weaponToEquip) =>
            SendAsync(nameof(IGameHub.EquipWeapon), tropperToEquip, weaponToEquip);

        public ValueTask EquipHelmetAsync(TrooperClass tropperToEquip, HelmetName helmetToEquip) =>
            SendAsync(nameof(IGameHub.EquipHelmet), tropperToEquip, helmetToEquip);

        public ValueTask EquipArmorAsync(TrooperClass tropperToEquip, ArmorName armorToEquip) =>
            SendAsync(nameof(IGameHub.EquipArmor), tropperToEquip, armorToEquip);

        public ValueTask LevelUpTrooperInstant(TrooperClass trooperClass) =>
            SendAsync(nameof(IGameHub.LevelUpTrooperInstant), trooperClass);

        public ValueTask LevelUpTrooperRegular(TrooperClass trooperClass) =>
            SendAsync(nameof(IGameHub.LevelUpTrooperRegular), trooperClass);

        public ValueTask GradeUpTrooperInstant(TrooperClass trooperClass) =>
            SendAsync(nameof(IGameHub.GradeUpTrooperInstant), trooperClass);

        public ValueTask LevelUpWeaponInstant(WeaponName name) =>
            SendAsync(nameof(IGameHub.LevelUpWeaponInstant), name);

        public ValueTask LevelUpWeaponRegular(WeaponName name) =>
            SendAsync(nameof(IGameHub.LevelUpWeaponRegular), name);

        public ValueTask GradeUpWeaponInstant(WeaponName name) =>
            SendAsync(nameof(IGameHub.GradeUpWeaponInstant), name);

        public ValueTask LevelUpHelmetInstant(HelmetName name) =>
            SendAsync(nameof(IGameHub.LevelUpHelmetInstant), name);

        public ValueTask LevelUpHelmetRegular(HelmetName name) =>
            SendAsync(nameof(IGameHub.LevelUpHelmetRegular), name);

        public ValueTask GradeUpHelmetInstant(HelmetName name) =>
            SendAsync(nameof(IGameHub.GradeUpHelmetInstant), name);

        public ValueTask LevelUpArmorInstant(ArmorName name) =>
            SendAsync(nameof(IGameHub.LevelUpArmorInstant), name);

        public ValueTask LevelUpArmorRegular(ArmorName name) =>
            SendAsync(nameof(IGameHub.LevelUpArmorRegular), name);

        public ValueTask GradeUpArmorInstant(ArmorName name) =>
            SendAsync(nameof(IGameHub.GradeUpArmorInstant), name);

        public ValueTask SyncArmyItemProgress() =>
            SendAsync(nameof(IGameHub.SyncArmyItemProgress));

        public ValueTask CompleteArmyItemProgress() =>
            SendAsync(nameof(IGameHub.CompleteArmyItemProgress));

        public ValueTask CollectRepeatingReward() =>
            SendAsync(nameof(IGameHub.CollectRepeatingReward));

        public ValueTask Cheat_UnlockContent() =>
            SendAsync(nameof(IGameHub.Cheat_UnlockContent));

        public ValueTask Cheat_LevelUpPlayer() =>
            SendAsync(nameof(IGameHub.Cheat_LevelUpPlayer));

        public ValueTask Cheat_AddResources(int cash, int gold) =>
            SendAsync(nameof(IGameHub.Cheat_AddResources), cash, gold);
    }
}