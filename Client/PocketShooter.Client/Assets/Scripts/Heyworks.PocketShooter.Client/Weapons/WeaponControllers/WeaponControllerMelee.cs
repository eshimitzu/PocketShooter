using Collections.Pooled;
using Heyworks.PocketShooter.Configuration;
using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Weapons
{
    public class WeaponControllerMelee : WeaponController
    {
        protected WeaponRaycasterMelee WeaponRaycasterMelee => WeaponRaycaster as WeaponRaycasterMelee;

        protected override bool TestIfEnemyIsInCrosshair()
        {
            MeleeWeaponInfo meleeWeaponConfig = (MeleeWeaponInfo)Weapon.Info;

            LastShotInfo.Clear();

            return WeaponRaycasterMelee.MeleeHit(LastShotInfo, meleeWeaponConfig.HitZoneWidth, meleeWeaponConfig.HitZoneHeight, meleeWeaponConfig.MaxRange);
        }

        protected override void Attack()
        {
            var hits = new PooledList<ShotInfo>();

            for (int i = 0; i < LastShotInfo.Count; i++)
            {
                if (LastShotInfo[i].Target)
                {
                    hits.Add(new ShotInfo(LastShotInfo[i].Target.Id, Weapon.Name, LastShotInfo[i].IsCritical));
                }
            }

            if (hits.Count > 0)
            {
                var commandData = new AttackCommandData(LocalCharacter.Model.Id, hits);
                LocalCharacter.AddCommand(commandData);
            }
        }
    }
}
