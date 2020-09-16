using ExitGames.Client.Photon;
using Microsoft.Extensions.Logging;

namespace Heyworks.PocketShooter.Realtime.Connection
{
    // just example how to log some relevant information with specified event id
    internal static class LoggerExtensions
    {
        public static void OnRealtimeServerConnected(this ILogger self, StatusCode newCode) =>
            self.LogInformation(NetLog.Connected, "Realtime server connected: statusCode = {statusCode}.", newCode);

        public static void OnRealtimeServerDisconnected(this ILogger self, StatusCode newCode) =>
            self.LogInformation(NetLog.Disconnected, "Realtime server disconnected: statusCode = {statusCode}.", newCode);

        public static void OnRealtimeConnectionStatusChanged(this ILogger self, StatusCode newCode) =>
            self.LogInformation("Realtime server connection status changed: statusCode = {statusCode}.", newCode);
    }
}