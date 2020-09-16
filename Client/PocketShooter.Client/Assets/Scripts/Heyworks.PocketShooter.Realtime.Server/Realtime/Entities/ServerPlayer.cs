using System;
using System.Collections.Generic;
using Collections.Pooled;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities.Skills;

namespace Heyworks.PocketShooter.Realtime.Entities
{
    /// <summary>
    /// Represents server player entity.
    /// </summary>
    public class ServerPlayer : OwnedPlayer
    {
        public ServerPlayer(IRef<PlayerState> playerStateRef, TrooperInfo trooperInfo, ConsumablesPlayerState consumablesState)
            : base(playerStateRef, trooperInfo)
        {
            var (medKitSkill, medKitSkillInfo) = GetFirstSkillWithInfo<ConsumableSkill, MedKitSkillInfo>(SkillName.MedKit);
            if (medKitSkill != null && medKitSkillInfo != null)
            {
                medKitSkill.AvailableCount = Math.Min(medKitSkillInfo.AvailablePerSpawn, consumablesState.TotalSupports);
            }

            var (grenadeSkill, grenadeSkillInfo) = GetFirstSkillWithInfo<ConsumableSkill, GrenadeSkillInfo>(SkillName.Grenade);
            if (grenadeSkill != null)
            {
                grenadeSkill.AvailableCount = Math.Min(grenadeSkillInfo.AvailablePerSpawn, consumablesState.TotalOffensives);
            }
        }

        public PooledList<ShotInfo> Shots => playerStateRef.Value.Shots;

        public PooledList<DamageInfo> Damages => playerStateRef.Value.Damages;

        public new PooledList<HealInfo> Heals => playerStateRef.Value.Heals;

        public override void ApplyState(IRef<PlayerState> playerStateRef)
        {
            base.ApplyState(playerStateRef);

            CurrentWeapon.ApplyState(playerStateRef);
        }
    }
}