using System;
using Collections.Pooled;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.Realtime.State;

namespace Heyworks.PocketShooter.Realtime.Data
{
    /// <summary>
    /// Represents player state.
    /// </summary>
    public struct PlayerState : IEntity<EntityId>, IDisposable
    {
        EntityId IEntity<EntityId>.Id => Id;

        /// <summary>
        /// Gets the player identifier.
        /// </summary>
        public EntityId Id;

        // TODO: v.shimkovich Remove name from realtime state, send remote player info on separate join command.
        public string Nickname;

        public RemoteServerEvents ServerEvents;

        public WeaponComponents Weapon;

        public FpsTransformComponent Transform;

        public EffectComponents Effects;

        /// <summary>
        /// Gets the trooper class.
        /// </summary>
        public TrooperClass TrooperClass;

        /// <summary>
        /// Gets player team.
        /// </summary>
        public TeamNo Team;

        /// <summary>
        /// Gets the health.
        /// </summary>
        public HealthComponent Health;

        /// <summary>
        /// Gets the armor.
        /// </summary>
        public ArmorComponent Armor;

        /// <summary>
        /// The first skill.
        /// </summary>
        public SkillComponents Skill1;

        /// <summary>
        /// The second skill.
        /// </summary>
        public SkillComponents Skill2;

        /// <summary>
        /// The third skill.
        /// </summary>
        public SkillComponents Skill3;

        /// <summary>
        /// The 4 skill.
        /// </summary>
        public SkillComponents Skill4;

        /// <summary>
        /// The 5 skill.
        /// </summary>
        public SkillComponents Skill5;

        /// <summary>
        /// Player shoot info.
        /// </summary>
        public PooledList<ShotInfo> Shots;

        /// <summary>
        /// Who and how much damage was inflicted onto this player.
        /// </summary>
        public PooledList<DamageInfo> Damages;

        /// <summary>
        /// Heals of the player.
        /// </summary>
        public PooledList<HealInfo> Heals;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerState" /> struct.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="nickname">Nickname.</param>
        /// <param name="trooperClass">The class.</param>
        /// <param name="team">The team.</param>
        /// <param name="fpsTransform">Player transform.</param>
        /// <param name="health">The health.</param>
        /// <param name="armor">The armor.</param>
        /// <param name="weapon">The weapon.</param>
        /// <param name="skill1">Skill1.</param>
        /// <param name="skill2">Skill2.</param>
        /// <param name="skill3">Skill3.</param>
        /// <param name="skill4">Skill4.</param>
        /// <param name="skill5">Skill5.</param>
        /// <param name="serverEvents">Server events.</param>
        /// <param name="shots">Player shots.</param>
        /// <param name="damages">Player damages.</param>
        public PlayerState(
            EntityId id,
            string nickname,
            TrooperClass trooperClass,
            TeamNo team,
            FpsTransformComponent fpsTransform,
            HealthComponent health,
            ArmorComponent armor,
            WeaponComponents weapon,
            SkillComponents skill1,
            SkillComponents skill2,
            SkillComponents skill3,
            SkillComponents skill4,
            SkillComponents skill5,
            EffectComponents effects,
            RemoteServerEvents serverEvents,
            PooledList<ShotInfo> shots,
            PooledList<DamageInfo> damages,
            PooledList<HealInfo> heals)
        {
            Id = id;
            Nickname = nickname;
            TrooperClass = trooperClass;
            Team = team;
            Transform = fpsTransform;
            Health = health;
            Armor = armor;
            Weapon = weapon;
            Skill1 = skill1;
            Skill2 = skill2;
            Skill3 = skill3;
            Skill4 = skill4;
            Skill5 = skill5;
            Shots = shots;
            Damages = damages;
            Heals = heals;
            Effects = effects;
            ServerEvents = serverEvents;
        }

        public static PlayerState Create(
            EntityId id,
            string nickname,
            TeamNo team,
            TrooperInfo trooperInfo,
            FpsTransformComponent transform)
        {
            var weaponInfo = trooperInfo.Weapon;

            var weaponComponents = new WeaponComponents()
            {
                Base = new WeaponBaseComponent { State = WeaponState.Default, Name = weaponInfo.Name },
                Consumable = new WeaponConsumableComponent { AmmoInClip = weaponInfo.ClipSize },
            };

            var skillComponents1 = new SkillComponents()
            {
                Base = new SkillBaseComponent { State = SkillState.Default, Name = trooperInfo.Skill1.Name },
            };

            var skillComponents2 = new SkillComponents()
            {
                Base = new SkillBaseComponent { State = SkillState.Default, Name = trooperInfo.Skill2.Name },
            };
            var skillComponents3 = new SkillComponents()
            {
                Base = new SkillBaseComponent { State = SkillState.Default, Name = trooperInfo.Skill3.Name },
            };
            var skillComponents4 = new SkillComponents()
            {
                Base = new SkillBaseComponent { State = SkillState.Default, Name = trooperInfo.Skill4.Name },
            };
            var skillComponents5 = new SkillComponents()
            {
                Base = new SkillBaseComponent { State = SkillState.Default, Name = trooperInfo.Skill5.Name },
            };

            var state = new PlayerState
            {
                Id = id,
                Nickname = nickname,
                TrooperClass = trooperInfo.Class,
                Team = team,
                Transform = transform,
                Shots = new PooledList<ShotInfo>(),
                Damages = new PooledList<DamageInfo>(),
                Heals = new PooledList<HealInfo>(),
                Health = new HealthComponent { MaxHealth = trooperInfo.MaxHealth, Health = trooperInfo.MaxHealth },
                Armor = new ArmorComponent { MaxArmor = trooperInfo.MaxArmor, Armor = trooperInfo.MaxArmor },
                Weapon = weaponComponents,
                Skill1 = skillComponents1,
                Skill2 = skillComponents2,
                Skill3 = skillComponents3,
                Skill4 = skillComponents4,
                Skill5 = skillComponents5,
            };

            return state;
        }

        internal static void CloneReferences(in PlayerState from, ref PlayerState to)
        {
            to.Shots = from.Shots != null ? new PooledList<ShotInfo>(from.Shots.Span) : null;
            to.Damages = from.Damages != null ? new PooledList<DamageInfo>(from.Damages.Span) : null;
            to.Heals = from.Heals != null ? new PooledList<HealInfo>(from.Heals.Span) : null;
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>The fully qualified type name.</returns>
        public override string ToString() =>
            $"{nameof(PlayerState)}{(Id, Transform, Weapon.Base)}, health: {Health.Health}, shots: {Shots?.Count},...";

        /// <inheritdoc/>
        public void Dispose()
        {
            Shots?.Dispose();
            Damages?.Dispose();
            Heals?.Dispose();
        }
    }

    public static class PlayerStateExtensions
    {
        /// <summary>
        /// Clones this instance.
        /// </summary>
        public static void Clone(this in PlayerState self, ref PlayerState to)
        {
            UnsafeClone.Clone(in self, ref to);
            PlayerState.CloneReferences(in self, ref to);
        }
    }
}