using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Communication;
using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Data;
using Heyworks.PocketShooter.Meta.Entities;
using Orleans;
using Orleans.Concurrency;

namespace Heyworks.PocketShooter.Meta.Services
{
    public class GameGrain : Grain, IGameGrain, IGameContentGrain
    {
        private readonly IGameFactory gameFactory;
        private readonly IConfigurationsProvider configurationsProvider;

        public GameGrain(IGameFactory gameFactory, IConfigurationsProvider configurationsProvider)
        {
            this.gameFactory = gameFactory;
            this.configurationsProvider = configurationsProvider;
        }

        public async Task CreatePlayerGame(Immutable<CreatePlayerData> data)
        {
            await GrainFactory.GetGrain<IPlayerGrain>(this.GetPrimaryKey()).Create(data);
            await GrainFactory.GetGrain<IArmyGrain>(this.GetPrimaryKey()).Create();

            var player = (await gameFactory.Create(this.GetPrimaryKey())).Player;

            var playerConfiguration = await configurationsProvider.GetPlayerConfiguration(player.Group);
            var reward = playerConfiguration.GetLevelUpReward(0, player.Level);

            await ApplyContent(reward.AsImmutable());
        }

        public async Task<ServerGameState> GetState()
        {
            var game = await gameFactory.Create(this.GetPrimaryKey());

            return game.GetState();
        }

        public async Task CheckRunnables()
        {
            var armyRunnablesGrain = GrainFactory.GetGrain<IArmyRunnablesGrain>(this.GetPrimaryKey());

            await armyRunnablesGrain.CheckRunnables();
        }

        public async Task ApplyContent(Immutable<IEnumerable<IContentIdentity>> content)
        {
            var playerContentGrain = GrainFactory.GetGrain<IPlayerContentGrain>(this.GetPrimaryKey());
            var armyContentGrain = GrainFactory.GetGrain<IArmyContentGrain>(this.GetPrimaryKey());

            foreach (var contentIdentity in content.Value)
            {
                switch (contentIdentity)
                {
                    case TrooperIdentity ti:
                        await armyContentGrain.AddTrooper(ti.AsImmutable());
                        break;
                    case WeaponIdentity wi:
                        await armyContentGrain.AddWeapon(wi.AsImmutable());
                        break;
                    case HelmetIdentity hi:
                        await armyContentGrain.AddHelmet(hi.AsImmutable());
                        break;
                    case ArmorIdentity ai:
                        await armyContentGrain.AddArmor(ai.AsImmutable());
                        break;
                    case ResourceIdentity ri:
                        await playerContentGrain.AddResource(ri.AsImmutable());
                        break;
                    case OffensiveIdentity oi:
                        await armyContentGrain.AddOffensive(oi.AsImmutable());
                        break;
                    case SupportIdentity si:
                        await armyContentGrain.AddSupport(si.AsImmutable());
                        break;
                    default:
                        throw new NotImplementedException($"The type of content identity {contentIdentity.GetType().Name} is not supported");
                }
            }
        }
    }
}
