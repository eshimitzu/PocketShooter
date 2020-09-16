using System;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Serialization;

namespace Heyworks.PocketShooter.Realtime.Service
{
    public class GameJoinedDataHandler : IDataHandler
    {
        private readonly DataSerializer<GameJoinedData> serializer = new GameJoinedDataSerializer();

        public event Action<GameJoinedData> GameJoined;

        public void Handle(NetworkData data)
        {
            GameJoined?.Invoke(serializer.Deserialize(data.Data));
        }
    }
}