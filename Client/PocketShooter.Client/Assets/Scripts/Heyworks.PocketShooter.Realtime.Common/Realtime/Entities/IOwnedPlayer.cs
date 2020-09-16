using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities.Weapon;

namespace Heyworks.PocketShooter.Realtime.Entities
{
    /// <summary>
    /// Represents readonly interface for owned readonly player entities.
    /// </summary>
    public interface IOwnedPlayer : IPlayer
    {
        IOwnedWeapon CurrentWeapon { get; }

        TrooperInfo Info { get; }

        bool CanUseFirstAidKit();

        bool CanAttack();

        bool IsCastingAnySkill();

        bool IsAimingWithSkill();

        bool CanUseSkill(SkillName skillName);

        T GetFirstSkillInfo<T>(SkillName skillName)
            where T : SkillInfo;
    }
}