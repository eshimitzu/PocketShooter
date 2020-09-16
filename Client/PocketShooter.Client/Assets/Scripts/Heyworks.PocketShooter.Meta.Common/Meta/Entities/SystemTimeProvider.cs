using System;

namespace Heyworks.PocketShooter.Meta.Entities
{
    public class SystemTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
