using System;
using System.Collections.Generic;
using System.Text;

namespace Heyworks.Realtime.Serialization.Data
{
    public struct CustomPrimitive
    {
        internal short value;
    }

    public struct LimitedCustomPrimitive
    {
        public const short MinValue = 0;
        public const short MaxValue = 64;
        internal short value;
    }
}
