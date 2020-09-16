using System;
using System.IO;
using System.Threading.Tasks;

namespace Heyworks.PocketShooter.Meta.Server
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            try
            {
                var metaServerBack = new MetaServerBack().Create(args, Directory.GetCurrentDirectory());
                await metaServerBack.StartAsync();

                var metaServerFront = new MetaServerFront().Create(args, Directory.GetCurrentDirectory());
                await metaServerFront.StartAsync();

                Console.WriteLine("Press Enter to terminate...");
                Console.ReadLine();

                await metaServerFront.StopAsync();
                await metaServerBack.StopAsync();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 1;
            }
        }
    }
}
