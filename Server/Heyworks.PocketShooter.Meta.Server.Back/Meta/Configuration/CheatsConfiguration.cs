using System.Collections.Generic;
using System.Linq;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Meta.Entities;

namespace Heyworks.PocketShooter.Meta.Configuration
{
    internal sealed class CheatsConfiguration : ICheatsConfiguration
    {
        private readonly ServerGameConfig gameConfig;

        public CheatsConfiguration(ServerGameConfig gameConfig)
        {
            this.gameConfig = gameConfig;
        }

        public IEnumerable<IContentIdentity> GetAllContent() =>
            gameConfig.ContentPacksConfig.SelectMany(_ => _.Content).ToList();
    }
}
