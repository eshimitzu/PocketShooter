using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Utils.Extensions;
using UnityEngine;

namespace Heyworks.PocketShooter.Character.Bot.Actions
{
    [TaskCategory("Bot")]
    public class BotCaptureZone : BotNavMeshMovement
    {
        private List<IZone> availibleZones = new List<IZone>();
        private IZone targetZone;

        public override void OnStart()
        {
            base.OnStart();

            availibleZones.Clear();

            foreach (IZone zone in Bot.Simulation.Game.Zones.Values)
            {
                if (!(zone.State.Captured && zone.State.OwnerTeam == Bot.Model.Team))
                {
                    availibleZones.Add(zone);
                }
            }

            if (availibleZones.Count > 0)
            {
                targetZone = availibleZones.RandomObject();
                RandomInZone(targetZone);
            }
        }

        private void RandomInZone(IZone zone)
        {
            var zonePos = new Vector3(targetZone.ZoneInfo.X, targetZone.ZoneInfo.Y, targetZone.ZoneInfo.Z);

            int tries = 5;
            while (tries-- > 0)
            {
                var rand = Random.insideUnitSphere;
                Vector3 destination = zonePos + new Vector3(rand.x, 0, rand.z) * Random.Range(0, zone.ZoneInfo.Radius);
                if (SamplePosition(destination))
                {
                    SetDestination(destination);
                    break;
                }
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (!(targetZone.State.Captured && targetZone.State.OwnerTeam == Bot.Model.Team))
            {
                if (HasArrived())
                {
                    RandomInZone(targetZone);
                }

                return TaskStatus.Running;
            }

            return TaskStatus.Success;
        }
    }
}
