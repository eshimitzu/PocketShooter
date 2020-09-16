using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Entities.Skills;
using Heyworks.PocketShooter.Realtime.Entities.Weapon.Systems;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public class ActivateInstantReloadSkillSystem : ActivateSkillSystem
    {
        public ActivateInstantReloadSkillSystem(ITicker ticker, SkillName skillName)
            : base(ticker, skillName)
        {
        }

        protected override bool Execute(OwnedPlayer initiator, OwnedSkill playerSkill, IGame game)
        {
            if (initiator.CurrentWeapon is IConsumableWeaponForSystem weapon &&
                weapon.AmmoInClip < weapon.Info.ClipSize)
            {
                bool isExecutable = base.Execute(initiator, playerSkill, game);

                if (isExecutable)
                {
                    weapon.AmmoInClip = weapon.Info.ClipSize;

                    return true;
                }
            }

            return false;
        }
    }
}
