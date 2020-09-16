using Heyworks.PocketShooter.Realtime.Entities.Skills;
using Heyworks.PocketShooter.Realtime.Entities.Weapon;

namespace Heyworks.PocketShooter.Realtime.Entities
{
    /// <summary>
    /// Represents readonly interface for remote readonly player entities.
    /// </summary>
    public interface IRemotePlayer : IVisualPlayer<IRemotePlayerEvents>
    {
        IWeapon CurrentWeapon { get; }

        Skill Skill1 { get; }

        Skill Skill2 { get; }

        Skill Skill3 { get; }

        Skill Skill4 { get; }

        Skill Skill5 { get; }
    }
}