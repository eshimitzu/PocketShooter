using System.Collections.Generic;
using System.Linq;
using Heyworks.PocketShooter.Realtime.DataHandlers;

namespace Heyworks.PocketShooter.Realtime
{
    public class DataDispatcher : IDataDispatcher
    {
        private readonly IList<IDataHandler> dataHandlers;

        public DataDispatcher() => dataHandlers = new List<IDataHandler>();

        public void AddDataHandler(IDataHandler handler) => 
            dataHandlers.Add(handler);

        public IDataHandler GetDataHandler(byte dataCode) => 
            dataHandlers.FirstOrDefault(item => item.CanHandleData(dataCode));

        public void RemoveDataHandler(IDataHandler handler) => dataHandlers.Remove(handler);
    }
}
