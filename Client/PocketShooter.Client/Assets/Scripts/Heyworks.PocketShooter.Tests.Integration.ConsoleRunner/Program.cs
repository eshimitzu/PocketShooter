using System;
using System.Threading.Tasks;
using Heyworks.PocketShooter.Tests;

namespace Heyworks.PocketShooter
{
    class Program
    {
        /// <summary>
        /// Runs tests.
        /// </summary>
        /// <param name="players">Number of players.</param>
        /// <param name="ticks">Number of ticks to play.</param>
        /// <param name="meta">Address of server.</param>
        /// <param name="cycles">How much match making and play cycles to make</param>
        static async Task Main(int players = 1, ushort ticks = 30, string meta = "127.0.0.1", int cycles = 1)
        {
            Console.WriteLine($"Will run as of {players} each doing {ticks} ticks after login into {meta}");
            Environment.SetEnvironmentVariable("POCKET_SHOOTER_META", meta);
            while (cycles > 0)
            {
                Console.WriteLine($"Starting {cycles} game round");
                var t = new GameHubTests();
                try
                {
                    await t.NewDeviceMatchMakeAndMove(players, ticks);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }

                cycles--;
                await Task.Delay(TimeSpan.FromSeconds(20));
            }
        }
    }
}
