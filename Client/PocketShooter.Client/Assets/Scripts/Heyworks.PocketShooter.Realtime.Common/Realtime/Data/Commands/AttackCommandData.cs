using System;
using System.Collections.Generic;
using Collections.Pooled;

namespace Heyworks.PocketShooter.Realtime.Data
{
    public struct AttackCommandData : IGameCommandData
    {
        public AttackCommandData(EntityId entityId, PooledList<ShotInfo> shots)
        {
            Shots = shots;
            ActorId = entityId;
        }

        public EntityId ActorId { get; }

        public SimulationDataCode Code => SimulationDataCode.Attack;

        public PooledList<ShotInfo> Shots { get; }

        public override string ToString() => $"{nameof(AttackCommandData)}{(ActorId, Code, Shots?.Count)}";
    }
}
