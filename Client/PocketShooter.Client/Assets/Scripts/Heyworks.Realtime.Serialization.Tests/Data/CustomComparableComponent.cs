using System;
using System.Collections.Generic;
using System.Text;

namespace Heyworks.Realtime.Serialization.Data
{
    struct CustomComparableComponent : IComparable<float>
    {
        public float a;

        // we use special meanint of comparison to ensure we are in range
        public int CompareTo(float other)
        {
            if (other == a) return 0;
            if (other - a > 0.001) return -1;
            if (other > a) return -2;
            return 100500;
        }
    }
}
