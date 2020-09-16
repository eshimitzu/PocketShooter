using System;
using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Orleans;
using Orleans.Concurrency;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public interface IConfigGrain : IGrainWithGuidKey
    {
        Task<Version> GetGameConfigVersion(string key);

        Task<Immutable<ServerGameConfig>> GetGameConfig(string key);
    }
}
