using Heyworks.PocketShooter.Realtime.Entities;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    internal interface IServerInitializeSystem : ISystem
    {
        void Initialize(IServerGame game);
    }
}