using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Entities.Skills;

namespace Heyworks.PocketShooter.Realtime.Systems
{
    public sealed class CheatSystem : IOwnerSystem
    {
        private readonly ITicker ticker;
        private readonly IServerGame game;
        private readonly CheatCommandData commandData;

        public CheatSystem(ITicker ticker, IServerGame game, CheatCommandData commandData)
        {
            this.ticker = ticker;
            this.game = game;
            this.commandData = commandData;
        }

        public bool Execute(OwnedPlayer player)
        {
            switch (commandData.CheatType)
            {
                case CheatType.StunSelf: StunSelf(player); break;
                case CheatType.RootSelf: RootSelf(player); break;
                case CheatType.KillSelf: KillSelf(player); break;
                case CheatType.FullArmor: FullArmor(player); break;
                case CheatType.NoArmor: NoArmor(player); break;
                case CheatType.ResetSkills: ResetSkills(player); break;
                case CheatType.OneHp: OneHp(player); break;
                case CheatType.Damage500: Damage500(player); break;
                case CheatType.EndGame: EndGame(); break;
            }

            return true;
        }

        private void StunSelf(OwnedPlayer player)
        {
            player.Effects.Stun.IsStunned = true;
            player.StunExpire.ExpireAt = ticker.Current + 60;
        }

        private void RootSelf(OwnedPlayer player)
        {
            player.Effects.Root.IsRooted = true;
            player.RootExpire.ExpireAt = ticker.Current + 60;
        }

        private void KillSelf(OwnedPlayer player)
        {
            player.Health.Health = 0;
            player.ServerEvents.LastKiller = player.Id;
        }

        private void FullArmor(OwnedPlayer player)
        {
            player.Armor.Armor = player.Armor.MaxArmor;
        }

        private void NoArmor(OwnedPlayer player)
        {
            player.Armor.Armor = 0;
        }

        private void ResetSkills(OwnedPlayer player)
        {
            ResetSkill(player.Skill1);
            ResetSkill(player.Skill2);
            ResetSkill(player.Skill3);
            ResetSkill(player.Skill4);
            ResetSkill(player.Skill5);
        }

        private void ResetSkill(OwnedSkill ownedSkill)
        {
            var skill = ownedSkill.Skill;
            skill.State = SkillState.Default;
            if (skill is ICooldownSkillForSystem cooldownSkill)
            {
                cooldownSkill.StateExpireAt = int.MaxValue;
            }

            if (skill is IConsumableSkillForSystem consumable)
            {
                switch (skill.Name)
                {
                    case SkillName.Grenade:
                        consumable.UseCount = 0;
                        break;
                    case SkillName.MedKit:
                        consumable.UseCount = 0;
                        break;
                }
            }
        }

        private void OneHp(OwnedPlayer player)
        {
            player.Health.Health = 1;
        }

        private void Damage500(OwnedPlayer player)
        {
            var damageSource = new EntityRef(EntityType.Weapon, player.CurrentWeapon.Id);
            player.StateRef.Value.Damages.Add(new DamageInfo(player.Id, damageSource, DamageType.Normal, 500f));
        }

        private void EndGame()
        {
            game.IsEnded = true;
        }
    }
}