using Heyworks.PocketShooter.Realtime.DataHandlers;

namespace Heyworks.PocketShooter.Realtime
{
    public interface IDataDispatcher
    {
        void AddDataHandler(IDataHandler handler);

        void RemoveDataHandler(IDataHandler handler);

        IDataHandler GetDataHandler(byte dataCode);
    }
}
