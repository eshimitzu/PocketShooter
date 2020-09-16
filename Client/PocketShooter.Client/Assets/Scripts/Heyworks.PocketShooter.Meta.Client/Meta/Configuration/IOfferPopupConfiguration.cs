using System.Collections.Generic;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Meta.Entities;

namespace Heyworks.PocketShooter.Meta.Configuration
{
    public interface IOfferPopupConfiguration
    {
        IReadOnlyList<OfferPopupData> GetOfferPopups();
    }
}
