using System;
using System.Threading.Tasks;

namespace Heyworks.PocketShooter.Meta.Entities
{
    public interface IGameFactory
    {
       Task<Game> Create(Guid playerId);
    }
}
