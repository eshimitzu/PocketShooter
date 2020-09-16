namespace Heyworks.PocketShooter.VersionUtility.Components
{
    public interface IEditorComponent
    {
        string GetComponentName();

        void OnEnable();

        void OnComponentGUI();

        void OnDisable();
    }
}