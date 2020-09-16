using Heyworks.PocketShooter.Modules.Analytics;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Simulation;

namespace Heyworks.PocketShooter.UI.TrooperSelection
{
    /// <summary>
    /// Trooper selection spawn handler.
    /// </summary>
    public class TrooperSelectionRespawnHandler : ITrooperSelectionHandler
    {
        private readonly IClientSimulation simulation;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="TrooperSelectionRespawnHandler"/> class.
        /// </summary>
        /// <param name="simulation">Simulation.</param>
        public TrooperSelectionRespawnHandler(IClientSimulation simulation)
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
