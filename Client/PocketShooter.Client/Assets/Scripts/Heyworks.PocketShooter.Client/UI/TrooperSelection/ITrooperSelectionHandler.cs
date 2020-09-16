namespace Heyworks.PocketShooter.UI.TrooperSelection
{
    /// <summary>
    /// Trooper selectable.
    /// </summary>
    public interface ITrooperSelectionHandler
    {
        /// <summary>
        /// Ons the selected.
        /// </summary>
        /// <param name="selectionParameters">Selection parameters.</param>
        void OnSelected(TrooperSelectionParameters selectionParameters);
    }
}