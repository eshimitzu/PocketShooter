using System;
using System.Threading.Tasks;

namespace Heyworks.PocketShooter.Meta.Entities
{
    public interface IPlayerFactory
    {
       Task<ServerPlayer> Create(Guid playerId);
    }
}
