using System;
using System.Threading;
using System.Threading.Tasks;

namespace Heyworks.PocketShooter.Tests
{
    public static class FuncBoolExtensions
    {
        public static async Task Poll(Func<bool> condition, TimeSpan timeout, string message = null)
        {
            var timeoutTracker = new TimeoutTracker(timeout);
            while (!condition())
            {
                await Task.Yield();
                if (timeoutTracker.IsExpired)
                {
                    if (message != null) Throw.Timeout(message);
                    Throw.Timeout();
                }
            }
        }

        public static async Task Poll(Func<Task<bool>> condition, TimeSpan timeout, string message = null)
        {
            var timeoutTracker = new TimeoutTracker(timeout);
            while (!await condition())
            {
                if (timeoutTracker.IsExpired)
                {
                    if (message != null) Throw.Timeout(message);
                    Throw.Timeout();
                }
            }
        }        

        public static async Task SpinWait(Action condition, TimeSpan timeout, string message = null)
        {
            var timeoutTracker = new TimeoutTracker(timeout);
            while (true)
            {
                condition();
                await Task.Yield();
                if (timeoutTracker.IsExpired)
                {
                    break;
                }
            }
        }
    }
}