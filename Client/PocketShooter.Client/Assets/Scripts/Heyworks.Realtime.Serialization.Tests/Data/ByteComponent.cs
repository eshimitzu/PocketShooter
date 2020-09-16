using System;
using System.Collections.Generic;
using System.Text;

namespace Heyworks.Realtime.Serialization.Data
{
    public struct ByteComponent
    {
        public ByteComponent(byte a) => this.a = a;
        public byte a;
    }
}
