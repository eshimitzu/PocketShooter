using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    internal sealed class WeaponDamageSystem : IServerInitiatorSystem
    {
        private readonly AttackCommandData commandData;

        // TODO: how to pass command into execute and still have generic API?
        public WeaponDamageSystem(AttackCommandData commandData)
        {
            this.commandData = commandData;
        }

        // TODO: v.shimkovich some skill (lifeDrain, regenOnKill) do the same job,
        // can be optimized (maybe run them only on the list of damages)
        // move rage out of system
        public bool Execute(ServerPlayer localPlayer, IServerGame game)
        {
            var weaponInfo = localPlayer.CurrentWeapon.Info;

            var shots = commandData.Shots.Span;
            for (var i = 0; i < shots.Length; i++)
            {
                ref readonly var shot = ref shots[i];
                localPlayer.Shots.Add(shot);
                var target = game.GetServerPlayer(shot.AttackedId);

                if (target.CannotBeDamaged(localPlayer.Team))
                {
                    continue;
                }

                var damage = weaponInfo.WeaponShotDamage(shot.IsHeadshot);

                var additionalDamagePercent = localPlayer.Effects.Rage.AdditionalDamagePercent;
                if (additionalDamagePercent > 0)
                {
                    damage += additionalDamagePercent * damage / 100f;
                }

                var damageSource = new EntityRef(EntityType.Weapon, localPlayer.CurrentWeapon.Id);
                var damageInfo = new DamageInfo(
                    localPlayer.Id,
                    damageSource,
                    shot.IsHeadshot ? DamageType.Critical : DamageType.Normal,
                    damage);

                target.Damages.Add(damageInfo);
            }

            return true;
        }
    }
}