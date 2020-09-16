using System;
using System.Collections.Generic;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Entities.Skills;
using Heyworks.PocketShooter.Skills.Configuration;
using UnityEngine;
using UnityEngine.Assertions;

namespace Heyworks.PocketShooter.Skills
{
    [CreateAssetMenu(fileName = "SkillFactory", menuName = "Heyworks/Skills/SkillController Factory")]
    public class SkillControllerFactory : ScriptableObject
    {
        [SerializeField]
        private List<SkillSpec> skillSpecs;

        public SkillVisualizer CreateSkillVisualizer(Skill skillModel, NetworkCharacter character)
        {
            SkillVisualizer visualizer;
            var skillName = skillModel.Name;
            var spec = GetSkillSpec(skillName);

            switch (skillName)
            {
                case SkillName.Grenade:
                    visualizer = new GrenadeSkillVisualizer((GrenadeSkill)skillModel, (GrenadeSkillSpec)spec, character);
                    break;
                case SkillName.RocketJump:
                    visualizer = new SkillVisualizer(skillModel, spec, character);
                    break;
                case SkillName.Invisibility:
                    visualizer = new InvisibilitySkillVisualizer(skillModel, (InvisibilitySkillSpec)spec, character);
                    break;
                case SkillName.Immortality:
                    visualizer = new ImmortalitySkillVisualizer(skillModel, (ImmortalitySkillSpec)spec, character);
                    break;
                case SkillName.Lucky:
                    visualizer = new LuckySkillVisualizer(skillModel, (LuckySkillSpec)spec, character);
                    break;
                case SkillName.ShockWave:
                    visualizer = new ShockWaveSkillVisualizer(skillModel, (ShockWaveSkillSpec)spec, character);
                    break;
                case SkillName.Dive:
                    visualizer = new DiveSkillVisualizer(skillModel, (DiveSkillSpec)spec, character);
                    break;
                case SkillName.ShockWaveJump:
                    visualizer = new ShockWaveJumpSkillVisualizer(skillModel, (ShockWaveJumpSkillSpec)spec, character);
                    break;
                case SkillName.Lifesteal:
                    visualizer = new LifestealSkillVisualizer((LifestealSkill)skillModel, (LifestealSkillSpec)spec, character);
                    break;
                case SkillName.Sprint:
                    visualizer = new SprintSkillVisualizer(skillModel, (SprintSkillSpec)spec, character);
                    break;
                case SkillName.InstantReload:
                    visualizer = new InstantReloadSkillVisualizer(skillModel, (InstantReloadSkillSpec)spec, character);
                    break;
                case SkillName.StealthDash:
                case SkillName.DoubleStealthDash:
                    visualizer = new StealthDashSkillVisualizer(skillModel, (StealthDashSkillSpec)spec, character);
                    break;
                case SkillName.MedKit:
                    visualizer = new MedKitSkillVisualizer(skillModel, (MedKitSkillSpec)spec, character);
                    break;
                case SkillName.StealthSprint:
                    visualizer = new StealthSprintSkillVisualizer(skillModel, (StealthSprintSkillSpec)spec, character);
                    break;
                case SkillName.Heal:
                    visualizer = new HealSkillVisualizer(skillModel, (HealSkillSpec)spec, character);
                    break;
                case SkillName.StunningHit:
                case SkillName.RootingHit:
                case SkillName.ExtraDamage:
                case SkillName.LifeDrain:
                case SkillName.Rage:
                case SkillName.RegenerationOnKill:
                case SkillName.ControlFreak:
                    visualizer = new SkillVisualizer(skillModel, spec, character);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return visualizer;
        }

        public SkillController CreateLocalSkillController(OwnedSkill skillModel, LocalCharacter character)
        {
            SkillController skillController;
            var skillName = skillModel.Name;
            var spec = GetSkillSpec(skillName);

            switch (skillName)
            {
                case SkillName.Dive:
                    skillController = new DiveSkillController(skillModel, spec, character);
                    break;
                case SkillName.Heal:
                    skillController = new HealSkillController(skillModel, spec, character);
                    break;
                case SkillName.RocketJump:
                    skillController = new JumpSkillController(skillModel, (JumpSkillSpec)spec, character);
                    break;
                case SkillName.ShockWave:
                    skillController = new ShockWaveSkillController(skillModel, spec, character);
                    break;
                case SkillName.ShockWaveJump:
                    skillController = new ShockWaveJumpSkillController(skillModel, (ShockWaveJumpSkillSpec)spec, character);
                    break;
                case SkillName.Sprint:
                case SkillName.StealthSprint:
                    skillController = new SprintSkillController(skillModel, spec, character);
                    break;
                case SkillName.StealthDash:
                case SkillName.DoubleStealthDash:
                    skillController = new StealthDashSkillController(skillModel, (StealthDashSkillSpec)spec, character);
                    break;
                case SkillName.InstantReload:
                case SkillName.Invisibility:
                case SkillName.Lifesteal:
                case SkillName.Lucky:
                case SkillName.MedKit:
                    skillController = new SkillController(skillModel, spec, character);
                    break;
                case SkillName.Grenade:
                    skillController = new GrenadeSkillController(skillModel, (GrenadeSkillSpec)spec, character);
                    break;
                case SkillName.StunningHit:
                case SkillName.RootingHit:
                case SkillName.ExtraDamage:
                case SkillName.LifeDrain:
                case SkillName.Rage:
                case SkillName.RegenerationOnKill:
                case SkillName.ControlFreak:
                case SkillName.Immortality:
                    skillController = new PassiveSkillController(skillModel, spec, character);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return skillController;
        }

        public SkillSpec GetSkillSpec(SkillName skillName)
        {
            var spec = skillSpecs.Find(f => f.SkillName == skillName);
            Assert.IsNotNull(spec, $"Can't find skill spec for skill {skillName}.");

            return spec;
        }
    }
}
