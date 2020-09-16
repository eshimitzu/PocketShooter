using Heyworks.PocketShooter.Character.Bot.Skills.Triggers;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Entities.Skills;
using Heyworks.PocketShooter.Skills;
using Heyworks.PocketShooter.Skills.Configuration;

namespace Heyworks.PocketShooter.Character.Bot.Skills
{
    public class BotSkillFactory
    {
        public static OwnedSkill GetSkill(ClientPlayer player, int id)
        {
            switch (id)
            {
                case 1:
                    return player.Skill1;
                case 2:
                    return player.Skill2;
                case 3:
                    return player.Skill3;
                case 4:
                    return player.Skill4;
                case 5:
                    return player.Skill5;
            }

            return null;
        }

        public static BotSkill CreateBotSkill(BotCharacter character, int id, SkillControllerFactory factory)
        {
            var player = character.Model as ClientPlayer;
            OwnedSkill ownedSkill = GetSkill(player, id);

            SkillName skillName = ownedSkill.Name;
            SkillSpec spec = factory.GetSkillSpec(skillName);
            BotSkill botSkill = null;

            switch (skillName)
            {
                case SkillName.Dive:
                    botSkill = new BotDiveSkill(ownedSkill, spec, character);
                    break;
                case SkillName.Heal:
                    botSkill = new BotSkill(ownedSkill, spec, character);
                    break;
                case SkillName.RocketJump:
                    botSkill = new BotJumpSkill(ownedSkill, (JumpSkillSpec)spec, character);
                    break;
                case SkillName.ShockWave:
                    botSkill = new BotShockWaveSkill(ownedSkill, spec, character);
                    break;
                case SkillName.ShockWaveJump:
                    botSkill = new BotShockWaveJumpSkill(ownedSkill, (ShockWaveJumpSkillSpec)spec, character);
                    break;
                case SkillName.Sprint:
                case SkillName.StealthSprint:
                    botSkill = new BotSprintSkill(ownedSkill, spec, character);
                    break;
                case SkillName.StealthDash:
                case SkillName.DoubleStealthDash:
                    botSkill = new BotStealthDashSkill(ownedSkill, (StealthDashSkillSpec)spec, character);
                    break;
                case SkillName.InstantReload:
                case SkillName.Invisibility:
                case SkillName.Lifesteal:
                case SkillName.Lucky:
                case SkillName.MedKit:
                    botSkill = new BotSkill(ownedSkill, spec, character);
                    break;
                case SkillName.Grenade:
                    botSkill = new BotGrenadeSkill(ownedSkill, (GrenadeSkillSpec)spec, character);
                    break;
                case SkillName.StunningHit:
                case SkillName.RootingHit:
                case SkillName.ExtraDamage:
                case SkillName.LifeDrain:
                case SkillName.Rage:
                case SkillName.RegenerationOnKill:
                case SkillName.ControlFreak:
                case SkillName.Immortality:
                    botSkill = null;
                    break;
                default:
                    botSkill = null;
                    break;
            }

            return botSkill;
        }

        public static BotSkillTrigger CreateTrigger(BotCharacter character, int id)
        {
            var player = character.Model as ClientPlayer;
            OwnedSkill ownedSkill = GetSkill(player, id);

            SkillName skillName = ownedSkill.Name;
            BotSkillTrigger skillTrigger = null;

            switch (skillName)
            {
                case SkillName.Dive:
                    skillTrigger = new BotAOESkillTrigger(character, ((AoESkillInfo)ownedSkill.Info).AoE.Radius, 1, 1, 0);
                    break;
                case SkillName.RocketJump:
                    skillTrigger = new BotSimpleSkillTrigger(character, 1, 1, 0);
                    break;
                case SkillName.ShockWave:
                    skillTrigger = new BotAOESkillTrigger(character, ((AoESkillInfo)ownedSkill.Info).AoE.Radius, 1, 1, 0);
                    break;
                case SkillName.ShockWaveJump:
                    skillTrigger = new BotAOESkillTrigger(character, ((AoESkillInfo)ownedSkill.Info).AoE.Radius, 1, 1, 0);
                    break;
                case SkillName.Sprint:
                    skillTrigger = new BotSimpleSkillTrigger(character, 1, 1, 0);
                    break;
                case SkillName.StealthSprint:
                    skillTrigger = new BotSimpleSkillTrigger(character, 1, 1, 0);
                    break;
                case SkillName.StealthDash:
                    skillTrigger = new BotSimpleSkillTrigger(character, 1, 0.2f, 0);
                    break;
                case SkillName.DoubleStealthDash:
                    skillTrigger = new BotSimpleSkillTrigger(character, 1, 0.2f, 0);
                    break;
                case SkillName.InstantReload:
                    skillTrigger = new BotSimpleSkillTrigger(character, 1, 0, 0);
                    break;
                case SkillName.Invisibility:
                    skillTrigger = new BotSimpleSkillTrigger(character, 0, 1, 0);
                    break;
                case SkillName.Lifesteal:
                    skillTrigger = new BotAOESkillTrigger(character, ((LifestealSkillInfo)ownedSkill.Info).AoE.Radius, 0, 0.5f, 1f);
                    break;
                case SkillName.Lucky:
                    skillTrigger = new BotSimpleSkillTrigger(character, 0, 2, 0);
                    break;
                case SkillName.Heal:
                case SkillName.MedKit:
                    skillTrigger = new BotSimpleSkillTrigger(character, 0, 0, 2);
                    break;
                case SkillName.Grenade:
                    skillTrigger = new BotTargetSkillTrigger(character, 8, 0.5f, 0, 0);
                    break;
                case SkillName.StunningHit:
                case SkillName.RootingHit:
                case SkillName.ExtraDamage:
                case SkillName.LifeDrain:
                case SkillName.Rage:
                case SkillName.RegenerationOnKill:
                case SkillName.ControlFreak:
                case SkillName.Immortality:
                    skillTrigger = null;
                    break;
                default:
                    skillTrigger = new BotSimpleSkillTrigger(character, 0, 0, 0);
                    break;
            }

            return skillTrigger;
        }
    }
}