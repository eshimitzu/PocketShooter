using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Hosting;

namespace Heyworks.PocketShooter
{
    public static class EnvironmentNames
    {
        public static string EnvironmentName
        {
            get
            {
                var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                return string.IsNullOrEmpty(environmentName) ? Development : environmentName;
            }
        }

        // ensures we have environment, which is Development if not defined
        public static string GetName(this IHostingEnvironment hostingEnvironment)
            => string.IsNullOrEmpty(hostingEnvironment.EnvironmentName) ? Development : hostingEnvironment.EnvironmentName;

        public static readonly string Development = Microsoft.Extensions.Hosting.EnvironmentName.Development;

        // database and meta and realtime may be deployed on same sever
        public static readonly string Integration = nameof(Integration);
        public static readonly string Testing = nameof(Testing);

        public static readonly string Staging = Microsoft.Extensions.Hosting.EnvironmentName.Staging;
        public static readonly string Production = Microsoft.Extensions.Hosting.EnvironmentName.Production;

        // database and meta and realtime considered to be deployed on separate servers
        public static bool IsStagingOrProduction(this IHostingEnvironment hostingEnvironment)
            => hostingEnvironment.IsStaging() || hostingEnvironment.IsProduction();

        // whole server could run out of the box with no config
        public static bool IsLocalOrDevelopment(this IHostingEnvironment hostingEnvironment) =>
            string.IsNullOrEmpty(hostingEnvironment.EnvironmentName)
            || hostingEnvironment.IsDevelopment(); // throws exception if null

        public static bool IsTesting(this IHostingEnvironment hostingEnvironment) => Testing.Equals(hostingEnvironment.EnvironmentName, StringComparison.InvariantCultureIgnoreCase);
    }
}
