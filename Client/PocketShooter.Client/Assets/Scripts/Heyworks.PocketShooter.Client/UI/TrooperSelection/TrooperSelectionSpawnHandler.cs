using Heyworks.PocketShooter.Modules.Analytics;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Simulation;

namespace Heyworks.PocketShooter.UI.TrooperSelection
{
    /// <summary>
    /// Trooper selection spawn handler.
    /// </summary>
    public class TrooperSelectionSpawnHandler : ITrooperSelectionHandler
    {
        private readonly IClientSimulation simulation;

        public TrooperSelectionSpawnHandler(IClientSimulation simulation)
        {
            this.simulation = simulation;
        }

        /// <inheritdoc/>
        public void OnSelected(TrooperSelectionParameters selectionParameters)
        {
            simulation.AddCommand(new SpawnTrooperCommandData(selectionParameters.TrooperClass));
        }
    }
}
