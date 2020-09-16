using Heyworks.PocketShooter.Configuration;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Simulation;
using Heyworks.PocketShooter.Weapons.AimAssistant;
using UnityEngine;

namespace Heyworks.PocketShooter.Weapons
{
    public class WeaponFireVisualiserMelee : WeaponFireVisualiser
    {
        private WeaponRaycasterMelee WeaponRaycasterMelee => (WeaponRaycasterMelee)WeaponRaycaster;

        protected override void ProcessShoot(AttackServerEvent ase)
        {
            base.ProcessShoot(ase);

            for (int i = 0; i < ase.Shots.Count; i++)
            {
                ShotInfo info = ase.Shots[i];

                if (info.AttackedId != 0)
                {
                    NetworkCharacter character = CharacterContainer.GetCharacter(info.AttackedId);
                    if (character)
                    {
                        AimAssistantTarget aim = character.AimAssistantTarget;
                        if (aim)
                        {
                            Collider aimCollider = aim.BodyCollider;
                            Vector3 position = WeaponRaycasterMelee.ShotOriginTransfrom.position;
                            Vector3 shotPoint = aimCollider.ClosestPoint(position);

                            VizualizeAttack(new Vector3[] { shotPoint });
                        }
                    }
                }
            }
        }
    }
}
