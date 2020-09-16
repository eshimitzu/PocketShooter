using System.Collections.Generic;
using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Entities;
using Orleans;
using Orleans.Concurrency;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public class CheatsGrain : Grain, ICheatsGrain
    {
        private readonly IPlayerFactory playerFactory;
        private readonly IConfigurationsProvider configurationsProvider;

        public CheatsGrain(IPlayerFactory playerFactory, IConfigurationsProvider configurationsProvider)
        {
            this.playerFactory = playerFactory;
            this.configurationsProvider = configurationsProvider;
        }

        public async Task UnlockContent()
        {
            var player = await playerFactory.Create(this.GetPrimaryKey());
            var cheatsConfiguration = await configurationsProvider.GetCheatsConfiguration(player.Group);

            var gameContentGrain = GrainFactory.GetGrain<IGameContentGrain>(this.GetPrimaryKey());

            await gameContentGrain.ApplyContent(cheatsConfiguration.GetAllContent().AsImmutable());
        }

        public async Task AddResources(int cash, int gold)
        {
            var gameContentGrain = GrainFactory.GetGrain<IGameContentGrain>(this.GetPrimaryKey());
            IEnumerable<IContentIdentity> content = new[] { new ResourceIdentity(cash, gold) };

            await gameContentGrain.ApplyContent(content.AsImmutable());
        }

        public async Task LevelUpPlayer()
        {
            var player = await playerFactory.Create(this.GetPrimaryKey());

            var playerConfiguration = await configurationsProvider.GetPlayerConfiguration(player.Group);
            var experienceForNextLevel = playerConfiguration.GetExperienceForNextLevel(player.Level, player.ExperienceInLevel);

            var playerContentGrain = GrainFactory.GetGrain<IPlayerContentGrain>(this.GetPrimaryKey());

            await playerContentGrain.AddExperience(experienceForNextLevel);
        }
    }
}
