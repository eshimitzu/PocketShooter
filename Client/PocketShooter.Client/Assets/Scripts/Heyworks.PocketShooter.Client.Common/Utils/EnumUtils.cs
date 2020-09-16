using System;
using System.Collections.Generic;
using System.Linq;

namespace Heyworks.PocketShooter.Utils
{
    public static class EnumUtils
    {
        public static IEnumerable<T> Values<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}