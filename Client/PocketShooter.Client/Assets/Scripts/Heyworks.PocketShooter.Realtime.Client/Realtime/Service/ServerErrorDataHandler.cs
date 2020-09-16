using System;
using Heyworks.PocketShooter.Realtime.Data;
using Microsoft.Extensions.Logging;

namespace Heyworks.PocketShooter.Realtime.Service
{
    public class ServerErrorDataHandler : IDataHandler
    {
        public event Action<ClientErrorCode> Error;

        public void Handle(NetworkData data)
        {
            var code = (ClientErrorCode)data.Data[0];
            Error?.Invoke(code);

            GameLog.Log.LogCritical("Unhandled exception on server with {code}.", code);
        }
    }
}