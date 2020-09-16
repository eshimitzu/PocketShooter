using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.UI.Core;
using UnityEngine;

namespace Heyworks.PocketShooter.UI.HUD
{
    public class DominationPointersPresenter : IDisposablePresenter
    {
        public DominationPointersPresenter(
            IClientGame game,
            TeamNo playerTeam,
            DominationModeInfo config,
            DominationModeView dominationModeView)
        {
            var index = 0;
            foreach (IZone zone in game.Zones.Values)
            {
                DominationZonePointer zonePointer = Object.Instantiate(
                    dominationModeView.ZonePointerPrefab,
                    dominationModeView.transform);
                zonePointer.Setup(index, zone, config, playerTeam);

                index++;
            }
        }

        public void Dispose()
        {
        }
    }
}