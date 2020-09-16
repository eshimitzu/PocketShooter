using System;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Serialization;

namespace Heyworks.PocketShooter.Realtime.Service
{
    public class BotControlTakenDataHandler : IDataHandler
    {
        private readonly DataSerializer<BotControlTakenData> serializer = new BotControlTakenDataSerializer();

        // bot was created on to be run by client which received this event
        public event Action<BotControlTakenData> BotControlTaken;

        public void Handle(NetworkData data)
        {
            BotControlTaken?.Invoke(serializer.Deserialize(data.Data));
        }
    }
}