using System;
using System.Net;
using Heyworks.PocketShooter.Utils;

namespace Heyworks.PocketShooter.Communication
{
    public readonly struct ServerAddress
    {
        public ServerAddress(string address) => Address = IPEndPointParser.Parse(address);

        public ServerAddress(string ip, int port) => Address = new IPEndPoint(IPAddress.Parse(ip), port);

        public IPEndPoint Address { get; }

        public override string ToString() => Address.ToString();
    }
}