using System;

namespace Heyworks.PocketShooter.Meta.Entities
{
    public interface IDateTimeProvider
    {
        DateTime UtcNow { get; }
    }
}
