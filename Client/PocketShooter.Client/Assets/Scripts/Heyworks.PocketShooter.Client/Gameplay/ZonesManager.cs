using System.Collections.Generic;
using Heyworks.PocketShooter.Modules.GameEnvironment;
using Heyworks.PocketShooter.Realtime;
using Heyworks.PocketShooter.Realtime.Entities;
using UnityEngine;

namespace Heyworks.PocketShooter.Gameplay
{
    internal class ZonesManager
    {
        private readonly ZoneView zonePrefab;

        private readonly List<ZoneView> zonesViews = new List<ZoneView>();

        internal ZonesManager(ZoneView zonePrefab)
        {
            this.zonePrefab = zonePrefab;
        }

        internal void SpawnZones(Room room)
        {
            foreach (var zone in room.Game.Zones.Values)
            {
                var zoneView = Object.Instantiate(zonePrefab);
                zoneView.transform.position = new Vector3(zone.ZoneInfo.X, zone.ZoneInfo.Y, zone.ZoneInfo.Z);
                zoneView.Setup(zone, room);

                zonesViews.Add(zoneView);
            }
        }

        internal void Dispose()
        {
            foreach (var zonesView in zonesViews)
            {
                Object.Destroy(zonesView.gameObject);
            }

            zonesViews.Clear();
        }
    }
}
