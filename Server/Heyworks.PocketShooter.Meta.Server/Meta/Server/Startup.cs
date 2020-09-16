using System;
using HealthChecks.UI.Client;
using Heyworks.PocketShooter.Meta.Communication;
using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.Meta.Serialization;
using Heyworks.PocketShooter.Meta.Server.ActionFilters;
using Heyworks.PocketShooter.Meta.Services;
using Heyworks.PocketShooter.Meta.Services.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Orleans;

namespace Heyworks.PocketShooter.Meta.Server
{
    public class Startup
    {
        private readonly MetaClusterClientBuilder clusterClientBuilder;

        public Startup(IConfiguration configuration, MetaClusterClientBuilder clusterClientBuilder)
        {
            Configuration = configuration;
            this.clusterClientBuilder = clusterClientBuilder;

            SecurityKey = new SymmetricSecurityKey(
                Guid.Parse(configuration.GetValue<string>("Security:AuthKey")).ToByteArray());

            BsonClassMap.RegisterClassMap<GoogleConnection>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<GameCenterConnection>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IAccountService, AccountService>();
            services.Configure<GoogleSocialOptions>(Configuration.GetSection("Social:Google"));
            services.AddSingleton<GoogleCredentialsValidator>();

            var userStorageConnection = Configuration.GetValue<string>("DataStorage:ConnectionString");
            var connectionUrl = MongoUrl.Create(userStorageConnection);
            services.AddSingleton<IMongoClient>(new MongoClient(connectionUrl));
            services.AddTransient(sp => sp.GetService<IMongoClient>().GetDatabase(connectionUrl.DatabaseName));
            services.AddTransient(sp => sp.GetService<IMongoDatabase>().GetCollection<User>("Users"));

            services.AddSingleton<CheckClientVersionFilter>();

            services
                .AddAuthentication(SecurityKey)
                .AddMvc(setup =>
                {
                    setup.Filters.AddService<CheckClientVersionFilter>();
                })
                .AddJsonOptions(options =>
                {
                    DefaultSerializerSettings.UpdateWithThisSettings(options.SerializerSettings);
                });

            services.AddHealthChecks()
                // TODO: add ping health check of cluster
                // .AddOrleansClientCheck(Configuration["Cluster:ClusterId"]);
                .AddMongoDb(Configuration["DataStorage:ConnectionString"]);

            services.AddMemoryCache();

            services
                .AddSignalR()
                .AddJsonProtocol(options =>
                {
                    DefaultSerializerSettings.UpdateWithThisSettings(options.PayloadSerializerSettings);
                });

            clusterClientBuilder.ConfigureApplicationParts(parts =>
            {
                parts.AddApplicationPart(typeof(IPlayerGrain).Assembly);
            });

            services.AddSingleton(clusterClientBuilder.Build());
            services.AddHostedService<MetaClusterClientService>();
            services.AddHostedService<GameBackgroundService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app
                .UseAuthentication()
                .UseExceptionHandler()
                .UseMvc()
                .UseSignalR(builder =>
                {
                    builder.MapHub<GameHub>($"/{nameof(IGameHub)}");
                })
                .UseHealthChecks("/health", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
        }

        internal static SymmetricSecurityKey SecurityKey { get; private set; }

        private IConfiguration Configuration { get; }
    }
}
