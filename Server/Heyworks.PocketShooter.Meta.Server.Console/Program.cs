using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Communication;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Server.SignalR
{
    public class Program
    {
        public static async Task Main(string[] args) 
        {
            var host = new SignalRServer().Create(args, Directory.GetCurrentDirectory(),null).Build();
            await host.StartAsync();
            Console.WriteLine("Press any key to terminate");
            Console.ReadKey();
            await host.StopAsync();
        }
    }
}
