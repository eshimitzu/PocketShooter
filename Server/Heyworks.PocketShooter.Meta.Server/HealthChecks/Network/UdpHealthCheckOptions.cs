using System;
using System.Collections.Generic;

namespace HealthChecks.Network
{
    public class UdpHealthCheckOptions
    {
        internal List<(string host, int port, byte[] request, TimeSpan timeout)> ConfiguredHosts = new List<(string host, int port, byte[] request, TimeSpan timeout)>();

        // may check all possible UDP protocols and put proper sequence for each of these
        // because on raw dummy data there could be no reponse
        // so this is poor man health check instead of using real client for UDP
        private byte[] defaultRequest = new byte[]{1};               
        
        /// <param name="request">
        /// if default, then send one byte with 1 in body,
        /// or run Wireshark -> Network for your IP addrss -> udp.dstport == <YOUR_PORT> -> run you client -> grab Data part of first message send -> put into C# array
        /// </param>
        /// <param name="timeout">Time to wait for responce. Default is 2 seconds. </param>        
        public UdpHealthCheckOptions AddHost(string host, int port, byte[] request = null, TimeSpan timeout = default)
        {
            if (timeout == default)
                timeout = TimeSpan.FromSeconds(2);
            if (request == null) request = defaultRequest;

            ConfiguredHosts.Add((host, port, request, timeout));
            return this;
        }
    }
}
