using Orleans.Services;

namespace Heyworks.PocketShooter.Meta.Services
{
    public interface IConfigurationServiceClient : IGrainServiceClient<IConfigurationGrainService>, IConfigurationGrainService
    {
    }
}
