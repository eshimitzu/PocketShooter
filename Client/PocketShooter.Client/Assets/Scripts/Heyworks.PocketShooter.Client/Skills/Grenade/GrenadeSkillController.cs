﻿using System.Collections;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities.Skills;
using Heyworks.PocketShooter.Realtime.Simulation;
using Heyworks.PocketShooter.Utils;
using Heyworks.PocketShooter.Utils.Extensions;
using UniRx;
using UnityEngine;

namespace Heyworks.PocketShooter.Skills
{
    public class GrenadeSkillController : SkillController
    {
        private new GrenadeSkillSpec Spec { get; }

        private readonly IAimingSkill grenadeSkill;

        private Coroutine aimingCoroutine;
        private TrajectoryDrawer trajectoryDrawer;
        private Transform rightHand;

        public GrenadeSkillController(OwnedSkill skillModel, GrenadeSkillSpec spec, LocalCharacter character)
            : base(skillModel, spec, character)
        {
            Spec = spec;
            grenadeSkill = (IAimingSkill)skillModel.Skill;

            character.ModelEvents.SkillAimChanged.Where(e => e.SkillName == SkillName.Grenade)
                .Subscribe(AimChanged)
                .AddTo(character);

            rightHand = Character.CharacterCommon.TrooperAvatar.Animator.GetBoneTransform(HumanBodyBones.RightHand);
            trajectoryDrawer = Object.Instantiate(Spec.TrajectoryPrefab, character.transform);
            trajectoryDrawer.gameObject.SetActive(false);
        }

        public override void SkillControlOnDown()
        {
            Character.AddCommand(new AimSkillCommandData(Character.Id, SkillName.Grenade, true));
        }

        public override void SkillControlOnUp()
        {
            Character.AddCommand(new AimSkillCommandData(Character.Id, SkillName.Grenade, false));
            Character.UseSkill(SkillName);
        }

        public override void Cancel()
        {
            aimingCoroutine?.StopCoroutine();
        }

        public override void SkillControlOnClick()
        {
        }

        private void AimChanged(SkillAimChangeEvent e)
        {
            if (e.IsAiming)
            {
                trajectoryDrawer.gameObject.SetActive(true);
                aimingCoroutine = AimingCoroutine().StartCoroutine();
            }
            else
            {
                aimingCoroutine?.StopCoroutine();
                trajectoryDrawer.gameObject.SetActive(false);
            }
        }

        private IEnumerator AimingCoroutine()
        {
            while (true)
            {
                var aimAngle = Character.Model.Transform.Pitch;

                Quaternion throwAngle = Quaternion.Euler(Spec.InitialThrowAngle + aimAngle, 0f, 0f);
                Vector3 force = (Character.transform.rotation * throwAngle) * new Vector3(0, 0, Spec.ThrowPower);
                trajectoryDrawer.Draw(rightHand.transform.position, force);

                yield return new WaitForEndOfFrame();
            }
        }
    }
}