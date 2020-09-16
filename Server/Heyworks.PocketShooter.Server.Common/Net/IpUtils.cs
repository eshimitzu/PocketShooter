using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Heyworks.PocketShooter.Net
{
    /// <summary>
    /// Utilities for working with IP addresses.
    /// </summary>
    public class IPUtils
    {
        private readonly HttpClient httpClient;
        private readonly ILogger logger;

        private readonly string[] lookupServiceUrls =
        {
            "https://checkip.amazonaws.com/",
            "https://ipinfo.io/ip",
        };

        public IPUtils(HttpClient httpClient, ILogger logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
        }

        /// <summary>
        /// Gets the public address of the server.
        /// </summary>
        /// <param name="publicIpAddressFromSettings">The optional address from the settings. If specified returns this address.</param>
        public Task<IPAddress> GetPublicIpAddress(string publicIpAddressFromSettings = null)
        {
            if (IPAddress.TryParse(publicIpAddressFromSettings, out var ipAddress))
            {
                return Task.FromResult(ipAddress);
            }

            return LookupPublicIpAddress();
        }

        /// <summary>
        /// Gets the addresses of the local server. Can be used for internal cluster netwokr communicaiton in same data center or in same VPN.
        /// </summary>
        /// <returns>The list of server's IPv4 addresses.</returns>
        public static IReadOnlyList<IPAddress> GetLocalIPAddresses(AddressFamily family = AddressFamily.InterNetwork, string interfaceName = null)
        {
            var loopback = (family == AddressFamily.InterNetwork) ? IPAddress.Loopback : IPAddress.IPv6Loopback;
            // get list of all network interfaces
            NetworkInterface[] netInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            var candidates = new List<IPAddress>();
            // loop through interfaces
            for (int i = 0; i < netInterfaces.Length; i++)
            {
                NetworkInterface netInterface = netInterfaces[i];

                if (netInterface.OperationalStatus != OperationalStatus.Up)
                {
                // Skip network interfaces that are not operational
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(interfaceName) &&
                    !netInterface.Name.StartsWith(interfaceName, StringComparison.Ordinal))
                {
                    continue;
                }

                bool isLoopbackInterface = (netInterface.NetworkInterfaceType == NetworkInterfaceType.Loopback);
                // get list of all unicast IPs from current interface
                UnicastIPAddressInformationCollection ipAddresses = netInterface.GetIPProperties().UnicastAddresses;

                // loop through IP address collection
                foreach (UnicastIPAddressInformation ip in ipAddresses)
                {
                    // Picking the first address of the requested family for now. Will need to revisit later
                    if (ip.Address.AddressFamily == family)
                    {
                        // don't pick loopback address, unless we were asked for a loopback interface
                        if (!(isLoopbackInterface && ip.Address.Equals(loopback)))
                        {
                            candidates.Add(ip.Address); // collect all candidates.
                        }
                    }
                }
            }

            return candidates;
        }

        /// <summary>
        /// Gets the address of the local server.
        /// If there are multiple addresses in the correct family in the server's DNS record, the "smallest" will be returned.
        /// </summary>
        /// <returns>The server's IPv4 address.</returns>
        internal IPAddress GetLocalIPAddress(AddressFamily family = AddressFamily.InterNetwork, string interfaceName = null)
        {
            var candidates = GetLocalIPAddresses(family, interfaceName);

            if (candidates.Count > 0)
            {
                return PickIPAddress(candidates);
            }

            throw new InvalidOperationException("Failed to get a local IP address.");
        }

        private async Task<IPAddress> LookupPublicIpAddress()
        {
            IPAddress result = null;

            if (lookupServiceUrls.Length == 0)
            {
                throw new InvalidOperationException("Could not lookup the public IP address: no Lookup Service URLs are defined.");
            }

            foreach (string url in lookupServiceUrls)
            {
                try
                {
                    result = await DoLookupPublicIpAddress(url);

                    logger.LogInformation("Public IP address: {ipAddress}, lookup at {lookupService}", result, url);

                    break;
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, ex.Message);

                    continue;
                }
            }

            if (result == null)
            {
                throw new InvalidOperationException("Could not retrieve the public IP address. Please make sure that internet access is available, or configure a fixed value for the PublicIPAddress in the configuration file.");
            }

            return result;
        }

        private async Task<IPAddress> DoLookupPublicIpAddress(string lookupServiceUrl)
        {
            string address;
            try
            {
                address = (await httpClient.GetStringAsync(lookupServiceUrl)).Trim();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to lookup public ip address at {lookupService}", lookupServiceUrl);
                throw;
            }

            if (IPAddress.TryParse(address, out var result))
            {
                return result;
            }
            else
            {
                throw new FormatException($"Failed to parse public ip address from {lookupServiceUrl}. Response = {address}");
            }
        }

        private IPAddress PickIPAddress(IReadOnlyList<IPAddress> candidates)
        {
            IPAddress chosen = null;
            foreach (IPAddress addr in candidates)
            {
                if (chosen == null)
                {
                    chosen = addr;
                }
                else
                {
                    // pick smallest address deterministically
                    if (CompareIPAddresses(addr, chosen))
                    {
                        chosen = addr;
                    }
                }
            }

            return chosen;
        }

        // returns true if lhs is "less" (in some repeatable sense) than rhs
        private bool CompareIPAddresses(IPAddress lhs, IPAddress rhs)
        {
            byte[] lbytes = lhs.GetAddressBytes();
            byte[] rbytes = rhs.GetAddressBytes();

            if (lbytes.Length != rbytes.Length)
            {
                return lbytes.Length < rbytes.Length;
            }

            // compare starting from most significant octet.
            // 10.68.20.21 < 10.98.05.04
            for (int i = 0; i < lbytes.Length; i++)
            {
                if (lbytes[i] != rbytes[i])
                {
                    return lbytes[i] < rbytes[i];
                }
            }

            // They're equal
            return false;
        }
    }
}
