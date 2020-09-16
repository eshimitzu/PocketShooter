using System;

namespace Heyworks.PocketShooter.Meta.Data
{
    public class ClientPlayerState : PlayerState
    {
        public ClientPlayerState(Guid id, string nickname, string deviceId, string group)
            : base(id, nickname, deviceId, group)
        {
        }
    }
}
