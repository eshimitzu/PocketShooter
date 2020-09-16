using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Simulation;
using Heyworks.PocketShooter.Weapons.AimAssistant;
using UnityEngine;

namespace Heyworks.PocketShooter.Weapons
{
    public class WeaponFireVisualizerFirearm : WeaponFireVisualiser
    {
        private WeaponRaycasterFirearm WeaponRaycasterFirearm => (WeaponRaycasterFirearm)WeaponRaycaster;

        protected override void ProcessShoot(AttackServerEvent ase)
        {
            base.ProcessShoot(ase);

            // check is this attack from current weapon
            for (int i = 0; i < ase.Shots.Count; i++)
            {
                ShotInfo info = ase.Shots[i];
                // bool visualized = false;

                if (info.AttackedId != 0)
                {
                    NetworkCharacter character = CharacterContainer.GetCharacter(info.AttackedId);
                    if (character)
                    {
                        AimAssistantTarget aim = character.AimAssistantTarget;
                        if (aim)
                        {
                            Collider aimCollider = info.IsHeadshot ? aim.HeadCollider : aim.BodyCollider;
                            Vector3 position = WeaponRaycasterFirearm.ShotOriginTransfrom.position;
                            Vector3 shotPoint = aimCollider.ClosestPoint(position);

                            Vector3 shootDir = shotPoint - position;
                            float distance = shootDir.magnitude;
                            if (distance > 0)
                            {
                                var ray = new Ray(position, shootDir.normalized);
                                LastShotInfo.Clear();
                                WeaponRaycasterFirearm.Shoot(ray, ray.direction.magnitude, 0, 1, 0, LastShotInfo);
                                if (LastShotInfo[0].Target != null)
                                {
                                    shotPoint = LastShotInfo[0].Point;
                                }
                            }

                            CharacterView.VizualizeAttack(new Vector3[] { shotPoint });
                            // visualized = true;
                        }
                    }
                }

                // NOTE: a.dezhurko Commented due to invalid configuration usage. Will be replaced by y.liavonchyck soon.

                // if (!visualized)
                // {
                //     var weaponConfig = Model.CurrentWeapon.Config;
                //     Vector3 position = WeaponRaycasterFirearm.ShotOriginTransfrom.position;
                //     var ray = new Ray(position, OrbitFollowTransform.transform.forward);
                //     WeaponRaycasterFirearm.Shoot(ray, weaponConfig.MaxRange, weaponConfig.FractionDispersion, 1, 0, LastShotInfo);
                //     VizualizeAttack(new[] { LastShotInfo[0].Point });
                // }
            }
        }
    }
}