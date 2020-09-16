using System.Threading;
using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.Meta.Serialization;
using Heyworks.PocketShooter.Realtime.Configuration.Data;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using Orleans.Runtime;

namespace Heyworks.PocketShooter.Meta.Services
{
    internal class BootstrapTask : IStartupTask
    {
        public Task Execute(CancellationToken cancellationToken)
        {
            // Map global convention to all BSON serialization
            var сonventionPack = new ConventionPack();
            сonventionPack.Add(new IgnoreExtraElementsConvention(true));
            ConventionRegistry.Register("ApplicationConventions", сonventionPack, t => true);

            BsonClassMap.RegisterClassMap<PaymentTransaction>(cm =>
            {
                cm.AutoMap();
                cm.SetIdMember(cm.GetMemberMap(c => c.Id));
                cm
                .IdMemberMap
                .SetSerializer(new StringSerializer(BsonType.ObjectId))
                .SetIdGenerator(StringObjectIdGenerator.Instance);
                cm.GetMemberMap(c => c.PlayerId).SetSerializer(new GuidSerializer(BsonType.String));
            });

            BsonClassMapExtensions.AutoMapWithSubtypes(typeof(PaymentTransaction));
            BsonClassMapExtensions.AutoMapWithSubtypes(typeof(IContentIdentity));

            BsonClassMapExtensions.AutoMapWithSubtypes(typeof(WeaponStatsConfig));
            BsonClassMapExtensions.AutoMapWithSubtypes(typeof(SkillStatsConfig));

            return Task.CompletedTask;
        }
    }
}
