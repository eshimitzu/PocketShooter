using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using Microsoft.Extensions.Hosting;

namespace Heyworks.PocketShooter.Meta.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
            var pathToContentRoot = Path.GetDirectoryName(pathToExe);
            Directory.SetCurrentDirectory(pathToContentRoot);

            // TODO: if Meta back run in same process as Front - do run it as background service or inherit-override WebHostService to stop it on request
            var metaServerBack = new MetaServerBack().Create(args, Directory.GetCurrentDirectory());
            metaServerBack.Start();

            var metaServerFront = new MetaServerFront().Create(args, Directory.GetCurrentDirectory());
            var metaServerFrontService = new WebHostService(metaServerFront);
            ServiceBase.Run(metaServerFrontService);
        }
    }
}
