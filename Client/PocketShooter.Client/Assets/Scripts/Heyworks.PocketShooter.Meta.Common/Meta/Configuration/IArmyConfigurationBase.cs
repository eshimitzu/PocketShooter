using System;
using Heyworks.PocketShooter.Meta.Entities;

namespace Heyworks.PocketShooter.Meta.Configuration
{
    public interface IArmyConfigurationBase
    {
        Price GetItemProgressCompletePrice(TimeSpan remainingTime);
    }
}
