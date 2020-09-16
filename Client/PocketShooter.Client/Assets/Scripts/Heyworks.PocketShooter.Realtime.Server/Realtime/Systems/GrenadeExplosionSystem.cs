using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    internal sealed class GrenadeExplosionSystem : IServerInitiatorSystem
    {
        private readonly GrenadeExplosionCommandData commandData;

        public GrenadeExplosionSystem(GrenadeExplosionCommandData commandData)
        {
            this.commandData = commandData;
        }

        public bool Execute(ServerPlayer initiator, IServerGame game)
        {
            var config = initiator.GetFirstSkillInfo<GrenadeSkillInfo>(SkillName.Grenade);

            var entity = new EntityRef(EntityType.Skill, (byte)SkillName.Grenade);

            for (var i = 0; i < commandData.DirectVictims.Count; i++)
            {
                var victim = commandData.DirectVictims[i];

                var target = game.GetServerPlayer(victim);
                if (target != null && initiator.Team != target.Team)
                {
                    target.Damages.Add(
                        new DamageInfo(
                            initiator.Id,
                            entity,
                            DamageType.Normal,
                            config.Damage));
                }
            }

            for (var i = 0; i < commandData.SplashVictims.Count; i++)
            {
                var victim = commandData.SplashVictims[i];

                var target = game.GetServerPlayer(victim);
                if (target != null && initiator.Team != target.Team)
                {
                    target.Damages.Add(
                        new DamageInfo(
                            initiator.Id,
                            entity,
                            DamageType.Splash,
                            config.SplashDamage));
                }
            }

            return true;
        }
    }
}