using System.IO;
using System.Threading;
using System;
using System.Net;
using System.Threading.Tasks;


namespace Heyworks.PocketShooter.Tests
{
    public struct Context
    {
        public int id;
        public string meta;

        public ushort totalTicks;

        public TextWriter Out;

        public byte network;
    }
}