namespace Heyworks.PocketShooter.Realtime.DataHandlers
{
    public interface IDataHandler
    {
        bool CanHandleData(byte dataCode);

        void HandleData(byte dataCode, byte[] data);
    }
}
