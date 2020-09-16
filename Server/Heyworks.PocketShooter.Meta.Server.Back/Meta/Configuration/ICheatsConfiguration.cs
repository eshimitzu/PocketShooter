using System.Collections.Generic;
using Heyworks.PocketShooter.Meta.Entities;

namespace Heyworks.PocketShooter.Meta.Configuration
{
    public interface ICheatsConfiguration
    {
        IEnumerable<IContentIdentity> GetAllContent();
    }
}
