using System;
using System.Linq;
using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Meta.Services;
using Heyworks.PocketShooter.Meta.Services.Purchases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Orleans;

namespace Heyworks.PocketShooter.Meta.Communication
{
    [Authorize]
    public class GameHub : Hub<IGameHubObserver>, IGameHub
    {
        private readonly IClusterClient clusterClient;
        private readonly IConfiguration configuration;
        private readonly IHostingEnvironment environment;

        public GameHub(IClusterClient clusterClient, IConfiguration configuration, IHostingEnvironment environment)
        {
            this.clusterClient = clusterClient;
            this.configuration = configuration;
            this.environment = environment;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            var request = Context.GetHttpContext()?.Request;
            if (request != null && request.Headers.TryGetValue(RequestHeaders.ClientVersion, out var clientVersion))
            {
                var acceptedClientVersions = configuration.GetSection("Meta:Front:AcceptedClientVersions").Get<string[]>();

                // if client version is accepted.
                if (acceptedClientVersions.Any(item => item == clientVersion))
                {
                    await SendGameState();

                    return;
                }
            }

            Context.Abort();
        }

        public async Task<ResponseOption<GameConfig>> SynchronizeConfig(Version configVersion)
        {
            var playerGrain = clusterClient.GetGrain<IPlayerGrain>(PlayerId);
            var group = await playerGrain.GetGroup();

            var configGrain = clusterClient.GetGrain<IConfigGrain>(Guid.Empty);
            var gameConfig = await configGrain.GetGameConfig(group);

            return gameConfig.Value.Version != configVersion
                ? ResponseOk.CreateOption(gameConfig.Value.ToGameConfig())
                : ResponseOk.CreateOption((GameConfig)null);
        }

        public async Task<ResponseOption> ChangeNickname(string newNickname)
        {
            var playerGrain = clusterClient.GetGrain<IPlayerGrain>(PlayerId);

            if (!await playerGrain.ChangeNickname(newNickname))
            {
                return ResponseError.CreateOption(ApiErrorCode.InvalidNickname);
            }

            return ResponseOk.CreateOption();
        }

        public async Task MakeMatch(MatchRequest requestedMatch)
        {
            var grain = clusterClient.GetGrain<IMatchMakingGrain>(Guid.Empty);
            await grain.FindRealtimeServer(PlayerId, requestedMatch);
        }

        public async Task<ResponseOption<MatchResultsData>> GetMatchResults(Guid roomId)
        {
            var matchResultsProvider = clusterClient.GetGrain<IMatchResultsProviderGrain>(roomId);
            var matchResults = await matchResultsProvider.GetMatchResults(PlayerId);

            return matchResults != null
                ? ResponseOk.CreateOption(matchResults)
                : ResponseError.CreateOption<MatchResultsData>(ApiErrorCode.MatchResultsNotFound);
        }

        /// <inheritdoc/>
        public async Task<ResponseOption> MakePurchase(string receiptData)
        {
            var shopGrain = clusterClient.GetGrain<IShopGrain>(PlayerId);
            try
            {
                await shopGrain.MakePurchase(receiptData);

                return ResponseOk.CreateOption();
            }
            catch (InvalidPaymentReceiptException)
            {
                return ResponseError.CreateOption(ApiErrorCode.InvalidPaymentReceipt);
            }
            catch (TransactionExistsException)
            {
                return ResponseError.CreateOption(ApiErrorCode.PaymentTransactionExists);
            }
        }

        public async Task BuyRosterProduct(string productId)
        {
            var shopGrain = clusterClient.GetGrain<IShopGrain>(PlayerId);

            if (!await shopGrain.BuyRosterProduct(productId))
            {
                await SendGameState();
            }
        }

        public async Task BuyShopProduct(string productId)
        {
            var shopGrain = clusterClient.GetGrain<IShopGrain>(PlayerId);

            if (!await shopGrain.BuyShopProduct(productId))
            {
                await SendGameState();
            }
        }

        public async Task EquipWeapon(TrooperClass trooperToEquip, WeaponName weaponToEquip)
        {
            var grain = clusterClient.GetGrain<IArmyGrain>(PlayerId);

            if (!await grain.EquipWeapon(trooperToEquip, weaponToEquip))
            {
                await SendGameState();
            }
        }

        public async Task EquipHelmet(TrooperClass trooperToEquip, HelmetName helmetToEquip)
        {
            var grain = clusterClient.GetGrain<IArmyGrain>(PlayerId);

            if (!await grain.EquipHelmet(trooperToEquip, helmetToEquip))
            {
                await SendGameState();
            }
        }

        public async Task EquipArmor(TrooperClass trooperToEquip, ArmorName armorToEquip)
        {
            var grain = clusterClient.GetGrain<IArmyGrain>(PlayerId);

            if (!await grain.EquipArmor(trooperToEquip, armorToEquip))
            {
                await SendGameState();
            }
        }

        public async Task LevelUpTrooperInstant(TrooperClass trooperClass)
        {
            var grain = clusterClient.GetGrain<IArmyGrain>(PlayerId);

            if (!await grain.LevelUpTrooperInstant(trooperClass))
            {
                await SendGameState();
            }
        }

        public async Task LevelUpTrooperRegular(TrooperClass trooperClass)
        {
            var grain = clusterClient.GetGrain<IArmyGrain>(PlayerId);

            if (!await grain.LevelUpTrooperRegular(trooperClass))
            {
                await SendGameState();
            }
        }

        public async Task GradeUpTrooperInstant(TrooperClass trooperClass)
        {
            var grain = clusterClient.GetGrain<IArmyGrain>(PlayerId);

            if (!await grain.GradeUpTrooperInstant(trooperClass))
            {
                await SendGameState();
            }
        }

        public async Task LevelUpWeaponInstant(WeaponName name)
        {
            var grain = clusterClient.GetGrain<IArmyGrain>(PlayerId);

            if (!await grain.LevelUpWeaponInstant(name))
            {
                await SendGameState();
            }
        }

        public async Task LevelUpWeaponRegular(WeaponName name)
        {
            var grain = clusterClient.GetGrain<IArmyGrain>(PlayerId);

            if (!await grain.LevelUpWeaponRegular(name))
            {
                await SendGameState();
            }
        }

        public async Task GradeUpWeaponInstant(WeaponName name)
        {
            var grain = clusterClient.GetGrain<IArmyGrain>(PlayerId);

            if (!await grain.GradeUpWeaponInstant(name))
            {
                await SendGameState();
            }
        }

        public async Task LevelUpHelmetInstant(HelmetName name)
        {
            var grain = clusterClient.GetGrain<IArmyGrain>(PlayerId);

            if (!await grain.LevelUpHelmetInstant(name))
            {
                await SendGameState();
            }
        }

        public async Task LevelUpHelmetRegular(HelmetName name)
        {
            var grain = clusterClient.GetGrain<IArmyGrain>(PlayerId);

            if (!await grain.LevelUpHelmetRegular(name))
            {
                await SendGameState();
            }
        }

        public async Task GradeUpHelmetInstant(HelmetName name)
        {
            var grain = clusterClient.GetGrain<IArmyGrain>(PlayerId);

            if (!await grain.GradeUpHelmetInstant(name))
            {
                await SendGameState();
            }
        }

        public async Task LevelUpArmorInstant(ArmorName name)
        {
            var grain = clusterClient.GetGrain<IArmyGrain>(PlayerId);

            if (!await grain.LevelUpArmorInstant(name))
            {
                await SendGameState();
            }
        }

        public async Task LevelUpArmorRegular(ArmorName name)
        {
            var grain = clusterClient.GetGrain<IArmyGrain>(PlayerId);

            if (!await grain.LevelUpArmorRegular(name))
            {
                await SendGameState();
            }
        }

        public async Task GradeUpArmorInstant(ArmorName name)
        {
            var grain = clusterClient.GetGrain<IArmyGrain>(PlayerId);

            if (!await grain.GradeUpArmorInstant(name))
            {
                await SendGameState();
            }
        }

        public Task SyncArmyItemProgress()
        {
            var grain = clusterClient.GetGrain<IArmyGrain>(PlayerId);

            return grain.SyncItemProgress();
        }

        public async Task CompleteArmyItemProgress()
        {
            var grain = clusterClient.GetGrain<IArmyGrain>(PlayerId);

            if (!await grain.CompleteItemProgress())
            {
                await SendGameState();
            }
        }

        public async Task CollectRepeatingReward()
        {
            var grain = clusterClient.GetGrain<IPlayerGrain>(PlayerId);

            if (!await grain.CollectRepeatingReward())
            {
                await SendGameState();
            }
        }

        public async Task Cheat_UnlockContent()
        {
            if (CanCheat)
            {
                var cheatsGrain = clusterClient.GetGrain<ICheatsGrain>(PlayerId);
                await cheatsGrain.UnlockContent();

                await SendGameState();
            }
        }

        public async Task Cheat_LevelUpPlayer()
        {
            if (CanCheat)
            {
                var cheatsGrain = clusterClient.GetGrain<ICheatsGrain>(PlayerId);
                await cheatsGrain.LevelUpPlayer();

                await SendGameState();
            }
        }

        public async Task Cheat_AddResources(int cash, int gold)
        {
            if (CanCheat)
            {
                var cheatsGrain = clusterClient.GetGrain<ICheatsGrain>(PlayerId);
                await cheatsGrain.AddResources(cash, gold);

                await SendGameState();
            }
        }

        private Guid PlayerId => Guid.Parse(Context.UserIdentifier);

        private bool CanCheat => environment.IsLocalOrDevelopment() || environment.IsTesting();

        private async Task SendGameState()
        {
            var gameGrain = clusterClient.GetGrain<IGameGrain>(PlayerId);
            var gameState = await gameGrain.GetState();

            await Clients.Caller.ReceiveGameState(gameState.ToClientState());
        }
    }
}