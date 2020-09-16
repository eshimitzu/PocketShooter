using System.Threading.Tasks;
using Orleans;
using Orleans.Concurrency;
using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public interface IBotsGrain : IGrainWithGuidKey
    {
        /// <summary>
        /// Gets specified number of bots with random troopers of specified level.
        /// If configuration is used then some modifiers and limits are applied to control game complexity.
        /// If configuration is absent for level, than no limits are applied.
        /// </summary>
        Task<Immutable<PlayerInfo[]>> GetBotPrototypes(int numberOfBots, int level, bool useConfiguration);
    }
}