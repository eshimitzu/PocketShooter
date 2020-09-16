using System;
using System.Collections.Generic;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities.Skills;
using Heyworks.PocketShooter.Realtime.Entities.Weapon;

namespace Heyworks.PocketShooter.Realtime.Entities
{
    /// <summary>
    /// Represents remote player in game simulation.
    /// </summary>
    public class RemotePlayer : PlayerBase, IRemotePlayer
    {
        private readonly RemoteWeapon currentWeapon;

        public RemotePlayerEvents Events { get; }

        public RemotePlayer(IRef<PlayerState> playerStateRef)
            : base(playerStateRef)
        {
            ref readonly var weapon = ref playerStateRef.Value.Weapon;
            currentWeapon = CreateWeapon(weapon.Base.Name);
            Events = new RemotePlayerEvents(Id);

            var skillRef1 = new SkillRef1(playerStateRef);
            Skill1 = SkillFactory.CreateSkill(skillRef1);

            var skillRef2 = new SkillRef2(playerStateRef);
            Skill2 = SkillFactory.CreateSkill(skillRef2);

            var skillRef3 = new SkillRef3(playerStateRef);
            Skill3 = SkillFactory.CreateSkill(skillRef3);

            var skillRef4 = new SkillRef4(playerStateRef);
            Skill4 = SkillFactory.CreateSkill(skillRef4);

            var skillRef5 = new SkillRef5(playerStateRef);
            Skill5 = SkillFactory.CreateSkill(skillRef5);
        }

        /// <summary>
        /// Gets the current weapon.
        /// </summary>
        public IWeapon CurrentWeapon => currentWeapon;

        IRemotePlayerEvents IVisualPlayer<IRemotePlayerEvents>.Events => Events;

        public Skill Skill1 { get; }

        public Skill Skill2 { get; }

        public Skill Skill3 { get; }

        public Skill Skill4 { get; }

        public Skill Skill5 { get; }

        /// <summary>
        /// Sets the player state.
        /// </summary>
        /// <param name="playerStateRef">The new state.</param>
        public override void ApplyState(IRef<PlayerState> playerStateRef)
        {
            base.ApplyState(playerStateRef);

            System.Diagnostics.Debug.Assert(
                playerStateRef.Value.Id == Id,
                "New state id differs from current state id.");

            this.playerStateRef = playerStateRef;

            currentWeapon.ApplyState(playerStateRef);

            Skill1.ApplyState(playerStateRef);
            Skill2.ApplyState(playerStateRef);
            Skill3.ApplyState(playerStateRef);
            Skill4.ApplyState(playerStateRef);
            Skill5.ApplyState(playerStateRef);
        }

        // TODO: a.dezhurko [weapon]: Move to factory.
        private RemoteWeapon CreateWeapon(WeaponName weaponName)
        {
            switch (weaponName)
            {
                case WeaponName.Knife:
                case WeaponName.Katana:
                case WeaponName.M16A4:
                case WeaponName.Remington:
                case WeaponName.SawedOff:
                case WeaponName.Bazooka:
                    return new RemoteWeapon(playerStateRef);
                case WeaponName.SVD:
                case WeaponName.Barret:
                case WeaponName.Minigun:
                    return new WarmingUpRemoteWeapon(playerStateRef);
                default:
                    throw new ArgumentOutOfRangeException(nameof(weaponName), weaponName, null);
            }
        }
    }
}