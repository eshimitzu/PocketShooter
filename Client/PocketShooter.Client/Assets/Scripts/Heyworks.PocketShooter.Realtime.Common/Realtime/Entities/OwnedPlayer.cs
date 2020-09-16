using System;
using System.Collections.Generic;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities.Skills;
using Heyworks.PocketShooter.Realtime.Entities.Weapon;

namespace Heyworks.PocketShooter.Realtime.Entities
{
    /// <summary>
    /// Represents owned (controlled and modified) player entity.
    /// </summary>
    public abstract class OwnedPlayer : PlayerBase, IOwnedPlayer
    {
        /// <summary>
        /// Gets the current weapon.
        /// </summary>
        public OwnedWeapon CurrentWeapon { get; }

        /// <summary>
        /// Gets the skill1.
        /// </summary>
        /// <value>The skill1.</value>
        public OwnedSkill Skill1 { get; }

        /// <summary>
        /// Gets the skill2.
        /// </summary>
        /// <value>The skill2.</value>
        public OwnedSkill Skill2 { get; }

        /// <summary>
        /// Gets the skill3.
        /// </summary>
        /// <value>The skill3.</value>
        public OwnedSkill Skill3 { get; }

        /// <summary>
        /// Gets the skill4.
        /// </summary>
        /// <value>The skill4.</value>
        public OwnedSkill Skill4 { get; }

        /// <summary>
        /// Gets the skill5.
        /// </summary>
        /// <value>The skill5.</value>
        public OwnedSkill Skill5 { get; }

        /// <summary>
        /// Gets trooper info.
        /// </summary>
        public TrooperInfo Info { get; }

        /// <summary>
        /// Gets current weapon.
        /// </summary>
        IOwnedWeapon IOwnedPlayer.CurrentWeapon => CurrentWeapon;

        internal ref MedKitOwnedComponent MedKit => ref playerStateRef.Value.Effects.MedKit.Owned;

        internal ref MedKitOwnedComponent Heal => ref playerStateRef.Value.Effects.Heal.Owned;

        internal ref StunExpireComponent StunExpire => ref playerStateRef.Value.Effects.Stun.Expire;

        internal ref RootExpireComponent RootExpire => ref playerStateRef.Value.Effects.Root.Expire;

        internal ref InvisibleExpireComponent InvisibleExpire => ref playerStateRef.Value.Effects.Invisible.Expire;

        internal ref LuckyExpireComponent LuckyExpire => ref playerStateRef.Value.Effects.Lucky.Expire;

        internal ref ImmortalityExpireComponent ImmortalExpire => ref playerStateRef.Value.Effects.Immortality.Expire;

        internal ref RageExpireComponent RageExpire => ref playerStateRef.Value.Effects.Rage.Expire;

        internal ref JumpExpireComponent JumpExpire => ref playerStateRef.Value.Effects.Jump.Expire;

        internal ref DashExpireComponent DashExpire => ref playerStateRef.Value.Effects.Dash.Expire;

        /// <summary>
        /// Initializes a new instance of the <see cref="OwnedPlayer" /> class.
        /// </summary>
        /// <param name="playerStateRef">The reference to player's state.</param>
        /// <param name="trooperInfo">The information about trooper.</param>
        protected OwnedPlayer(IRef<PlayerState> playerStateRef, TrooperInfo trooperInfo)
            : base(playerStateRef)
        {
            trooperInfo.NotNull();
            Info = trooperInfo;

            ref readonly var weapon = ref playerStateRef.Value.Weapon;
            CurrentWeapon = CreateWeapon(in weapon, trooperInfo.Weapon);

            var skillRef1 = new SkillRef1(playerStateRef);
            Skill1 = new OwnedSkill(SkillFactory.CreateSkill(skillRef1), trooperInfo.Skill1);

            var skillRef2 = new SkillRef2(playerStateRef);
            Skill2 = new OwnedSkill(SkillFactory.CreateSkill(skillRef2), trooperInfo.Skill2);

            var skillRef3 = new SkillRef3(playerStateRef);
            Skill3 = new OwnedSkill(SkillFactory.CreateSkill(skillRef3), trooperInfo.Skill3);

            var skillRef4 = new SkillRef4(playerStateRef);
            Skill4 = new OwnedSkill(SkillFactory.CreateSkill(skillRef4), trooperInfo.Skill4);

            var skillRef5 = new SkillRef5(playerStateRef);
            Skill5 = new OwnedSkill(SkillFactory.CreateSkill(skillRef5), trooperInfo.Skill5);
        }

        /// <summary>
        /// Apply state ref.
        /// </summary>
        /// <param name="playerState">Player state ref.</param>
        public override void ApplyState(IRef<PlayerState> playerState)
        {
            base.ApplyState(playerState);

            Skill1.Skill.ApplyState(playerState);
            Skill2.Skill.ApplyState(playerState);
            Skill3.Skill.ApplyState(playerState);
            Skill4.Skill.ApplyState(playerState);
            Skill5.Skill.ApplyState(playerState);
        }

        /// <summary>
        /// Returns if player can use first aid kit.
        /// </summary>
        public bool CanUseFirstAidKit()
        {
            return Health.Health < Health.MaxHealth && CanUseSkill(SkillName.MedKit);
        }

        /// <summary>
        /// Returns if player can attack.
        /// </summary>
        public bool CanAttack()
        {
            return
                IsAlive
                && !Effects.Stun.IsStunned
                && CurrentWeapon.State == WeaponState.Default
                && (!(CurrentWeapon is IWarmingUpWeapon warmUpWeapon) || warmUpWeapon.WarmupProgress >= 1)
                && (!(CurrentWeapon is IConsumableWeapon weapon) || weapon.AmmoInClip > 0)
                && !IsCastingAnySkill()
                && !IsAimingWithSkill();
        }

        // Check if casting skill
        public bool IsCastingAnySkill() =>
            ((Skill1.Skill is ICastableSkill castingSkill1) && castingSkill1.Casting) ||
            ((Skill2.Skill is ICastableSkill castingSkill2) && castingSkill2.Casting) ||
            ((Skill3.Skill is ICastableSkill castingSkill3) && castingSkill3.Casting) ||
            ((Skill4.Skill is ICastableSkill castingSkill4) && castingSkill4.Casting) ||
            ((Skill5.Skill is ICastableSkill castingSkill5) && castingSkill5.Casting);

        public bool IsAimingWithSkill() =>
            ((Skill1.Skill is IAimingSkill aimingSkill1) && aimingSkill1.Aiming) ||
            ((Skill2.Skill is IAimingSkill aimingSkill2) && aimingSkill2.Aiming) ||
            ((Skill3.Skill is IAimingSkill aimingSkill3) && aimingSkill3.Aiming) ||
            ((Skill4.Skill is IAimingSkill aimingSkill4) && aimingSkill4.Aiming) ||
            ((Skill5.Skill is IAimingSkill aimingSkill5) && aimingSkill5.Aiming);

        /// <summary>
        /// Returns if player can use skill.
        /// </summary>
        /// <param name="skillName">Skill name.</param>
        public bool CanUseSkill(SkillName skillName)
        {
            var skill = GetFirstSkill(skillName);

            if (skill != null)
            {
                return IsAlive && !Effects.Stun.IsStunned && skill.CanUseSkill();
            }

            return false;
        }

        /// <summary>
        /// Gets owned skill by name.
        /// </summary>
        /// <param name="skillName">Skill name.</param>
        public OwnedSkill GetFirstOwnedSkill(SkillName skillName)
        {
            if (Skill1.Name == skillName)
            {
                return Skill1;
            }

            if (Skill2.Name == skillName)
            {
                return Skill2;
            }

            if (Skill3.Name == skillName)
            {
                return Skill3;
            }

            if (Skill4.Name == skillName)
            {
                return Skill4;
            }

            if (Skill5.Name == skillName)
            {
                return Skill5;
            }

            return null;
        }

        /// <summary>
        /// Get first skill with information for provided name and casts it to the provided types.
        /// </summary>
        /// <typeparam name="TSkill">Type of skill.</typeparam>
        /// <typeparam name="TSkillInfo">Type of skill info.</typeparam>
        /// <param name="skillName">Skill name.</param>
        public (TSkill, TSkillInfo) GetFirstSkillWithInfo<TSkill, TSkillInfo>(SkillName skillName)
            where TSkill : class, ISkillForSystem
            where TSkillInfo : SkillInfo
        {
            var ownedSkill = GetFirstOwnedSkill(skillName);

            if (ownedSkill != null)
            {
                return ((TSkill)(ISkillForSystem)ownedSkill.Skill, (TSkillInfo)ownedSkill.Info);
            }

            return (null, null);
        }

        /// <summary>
        /// Returns information of first skill with provided name.
        /// </summary>
        /// <typeparam name="T">Type of information to cast.</typeparam>
        /// <param name="skillName">Skill name.</param>
        public T GetFirstSkillInfo<T>(SkillName skillName)
            where T : SkillInfo
        {
            var ownedSkill = GetFirstOwnedSkill(skillName);

            return (T)ownedSkill?.Info;
        }

        /// <summary>
        /// Returns first skill with provided name.
        /// </summary>
        /// <param name="skillName">Skill name.</param>
        public Skill GetFirstSkill(SkillName skillName)
        {
            var ownedSkill = GetFirstOwnedSkill(skillName);

            return ownedSkill?.Skill;
        }

        // TODO: a.dezhurko [weapon]: Move to factory.
        private OwnedWeapon CreateWeapon(in WeaponComponents weapon, WeaponInfo weaponInfo)
        {
            switch (weapon.Base.Name)
            {
                case WeaponName.Knife:
                case WeaponName.Katana:
                    return new NoAmmoOwnedWeapon(playerStateRef, weaponInfo);
                case WeaponName.M16A4:
                case WeaponName.Bazooka:
                case WeaponName.Remington:
                case WeaponName.SawedOff:
                case WeaponName.Minigun:
                    return new ConsumableOwnedWeapon(playerStateRef, weaponInfo);
                case WeaponName.SVD:
                case WeaponName.Barret:
                    return new WarmingUpConsumableOwnedWeapon(playerStateRef, (WarmingUpWeaponInfo)weaponInfo);
                default:
                    throw new ArgumentOutOfRangeException(nameof(weapon), weapon.Base.Name, null);
            }
        }
    }
}