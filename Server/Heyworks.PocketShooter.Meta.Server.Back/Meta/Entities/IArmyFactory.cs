using System;
using System.Threading.Tasks;

namespace Heyworks.PocketShooter.Meta.Entities
{
    public interface IArmyFactory
    {
       Task<ServerArmy> Create(ServerPlayer player);
    }
}
