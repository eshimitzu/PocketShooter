using System;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Serialization;

namespace Heyworks.PocketShooter.Realtime.Service
{
    public class GameEndedDataHandler : IDataHandler
    {
        private readonly DataSerializer<GameEndedData> serializer = new GameEndedDataSerializer();

        public event Action<GameEndedData> GameEnded;

        public void Handle(NetworkData data) => GameEnded?.Invoke(serializer.Deserialize(data.Data));
    }
}