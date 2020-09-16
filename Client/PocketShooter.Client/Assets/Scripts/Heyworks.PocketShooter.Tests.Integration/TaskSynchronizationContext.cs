using System.Threading;
using System.Threading.Tasks;

namespace Heyworks.PocketShooter.Tests
{
    public class TaskSynchronizationContext : SynchronizationContext
    {
        public override void Post(SendOrPostCallback d, object state) => Task.Run(() => d(state));
        public override void Send(SendOrPostCallback d, object state) => Task.Run(() => d(state));
    }
}