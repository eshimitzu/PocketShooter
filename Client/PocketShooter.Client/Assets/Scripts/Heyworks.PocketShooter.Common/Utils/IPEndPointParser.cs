using System;
using System.Net;

namespace Heyworks.PocketShooter.Utils
{
    /// <summary>
    /// A parser for endpoint addresses.
    /// </summary>
    public static class IPEndPointParser
    {
        /// <summary>
        /// Determines whether a string is a valid endpoint address.
        /// </summary>
        /// <param name="addressWithPort">The endpoint address string with port to validate.</param>
        /// <param name="endpoint">The <see cref="IPEndPoint"/> version of the string.</param>
        /// <returns><c>true</c> if <paramref name="addressWithPort"/> was able to be parsed as an endpoint address; otherwise, <c>false</c>.</returns>
        public static bool TryParse(string addressWithPort, out IPEndPoint endpoint)
        {
            string addressPart = null;
            string portPart = null;
            IPAddress address;
            endpoint = null;

            if (string.IsNullOrEmpty(addressWithPort))
            {
                return false;
            }

            var lastColonIndex = addressWithPort.LastIndexOf(':');
            if (lastColonIndex > 0)
            {
                // IPv4 with port or IPv6
                var closingIndex = addressWithPort.LastIndexOf(']');
                if (closingIndex > 0)
                {
                    // IPv6 with brackets
                    addressPart = addressWithPort.Substring(1, closingIndex - 1);
                    if (closingIndex < lastColonIndex)
                    {
                        // IPv6 with port [::1]:80
                        portPart = addressWithPort.Substring(lastColonIndex + 1);
                    }
                }
                else
                {
                    // IPv6 without port or IPv4
                    var firstColonIndex = addressWithPort.IndexOf(':');
                    if (firstColonIndex != lastColonIndex)
                    {
                        // IPv6 ::1
                        addressPart = addressWithPort;
                    }
                    else
                    {
                        // IPv4 with port 127.0.0.1:123
                        addressPart = addressWithPort.Substring(0, firstColonIndex);
                        portPart = addressWithPort.Substring(firstColonIndex + 1);
                    }
                }
            }
            else
            {
                // IPv4 without port
                addressPart = addressWithPort;
            }

            if (IPAddress.TryParse(addressPart, out address))
            {
                if (portPart != null)
                {
                    int port;
                    if (int.TryParse(portPart, out port))
                    {
                        endpoint = new IPEndPoint(address, port);
                        return true;
                    }
                    return false;
                }
                endpoint = new IPEndPoint(address, 0);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Converts an endpoint address with port string to an <see cref="IPEndPoint"/> instance.
        /// </summary>
        /// <param name="addressWithPort">The endpoint address string with port.</param>
        /// <exception cref="ArgumentNullException"><paramref name="addressWithPort"/> is null.</exception>
        /// <exception cref="FormatException"><paramref name="addressWithPort"/> is not a valid endpoint address.</exception>
        public static IPEndPoint Parse(string addressWithPort)
        {
            if (addressWithPort == null)
            {
                throw new ArgumentNullException(nameof(addressWithPort));
            }

            if (!TryParse(addressWithPort, out var endpoint))
            {
                throw new FormatException("The provided endpoint address string has an invalid format");
            }

            return endpoint;
        }
    }
}
