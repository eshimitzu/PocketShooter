using Heyworks.PocketShooter.Audio;
using Heyworks.PocketShooter.Character;
using Heyworks.PocketShooter.Configuration;
using Heyworks.PocketShooter.Modules.EffectsManager;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities.Skills;
using Heyworks.PocketShooter.Realtime.Simulation;
using Heyworks.PocketShooter.Skills.Configuration;
using UniRx;
using UnityEngine;

namespace Heyworks.PocketShooter.Skills
{
    public class StealthDashSkillController : SkillController, IFreezer
    {
        private readonly StealthDashSkillInfo config;
        private readonly ICastableSkill skill;

        // Effect only visible on for local player.
        private GameObject dashEffect;

        private new StealthDashSkillSpec Spec { get; }

        public StealthDashSkillController(OwnedSkill skillModel, StealthDashSkillSpec spec, LocalCharacter character)
            : base(skillModel, spec, character)
        {
            config = (StealthDashSkillInfo)skillModel.Info;
            skill = (ICastableSkill)skillModel.Skill;
            Spec = spec;

            character.ModelEvents.DashChanged.Subscribe(DashChanged).AddTo(character);
            character.ModelEvents.SkillCastChanged.Where(
                    e => e.SkillName == SkillName)
                .Subscribe(CastChanged)
                .AddTo(character);
            character.InputController.AddFreezer(this);
        }

        public bool FreezeMotion => skill.Casting || Character.Model.Effects.Dash.IsDashing;

        public bool FreezeRotation => skill.Casting || Character.Model.Effects.Dash.IsDashing;

        public override void Cancel()
        {
            EffectsManager.Instance.StopEffect(dashEffect, false);
        }

        private void CastChanged(SkillCastChangedEvent e)
        {
            if (Character.Model.Effects.Dash.IsDashing && !e.IsCasting)
            {
                dashEffect = EffectsManager.Instance.PlayEffect(EffectType.Dash, Character.OrbitCamera.transform);

                Character.CharacterController.Dash(
                    config.Length,
                    config.Speed,
                    // redundant?
                    Character.InputController.OrbitCamera.Transform.rotation);
            }
        }

        private void DashChanged(bool isDashing)
        {
            if (!isDashing)
            {
                EffectsManager.Instance.StopEffect(dashEffect, false);
            }
        }
    }
}