using Heyworks.PocketShooter.Realtime.Entities;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    internal interface IServerGameSystem : ISystem
    {
        bool Execute(IServerGame game);
    }
}