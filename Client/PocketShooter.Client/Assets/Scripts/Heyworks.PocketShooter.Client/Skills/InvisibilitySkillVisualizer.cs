using System.Collections.Generic;
using Heyworks.PocketShooter.Audio;
using Heyworks.PocketShooter.Character;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Entities.Skills;
using KinematicCharacterController;
using UniRx;
using UnityEngine;

namespace Heyworks.PocketShooter.Skills
{
    public class InvisibilitySkillVisualizer : SkillVisualizer
    {
        private readonly InvisibilitySkillSpec spec;

        private int backupLayer;
        private bool isAppliedInvisibleMaterial;

        public InvisibilitySkillVisualizer(Skill skillModel, InvisibilitySkillSpec spec, NetworkCharacter character)
            : base(skillModel, spec, character)
        {
            this.spec = spec;
            character.ModelEvents.InvisibleChanged.Subscribe(InvisibleChanged).AddTo(character);
        }

        public override void Cancel()
        {
            if (isAppliedInvisibleMaterial)
            {
                isAppliedInvisibleMaterial = false;
                SetLayer(backupLayer);
                RevertMaterials();
                Character.CharacterCommon.AudioController.PostEvent(AudioKeys.Event.PlayInvisibility);
            }
        }

        private void InvisibleChanged(bool isInvisible)
        {
            if (isInvisible)
            {
                if (!isAppliedInvisibleMaterial)
                {
                    isAppliedInvisibleMaterial = true;
                    backupLayer = Character.gameObject.layer;
                    SetLayer(spec.InvisibleLayer);
                    ApplyInvisibilityMaterial(Character.CharacterCommon.TrooperAvatar.RenderView);
                    ApplyInvisibilityMaterial(Character.CharacterView.WeaponView.RenderView);
                    Character.CharacterCommon.AudioController.PostEvent(AudioKeys.Event.PlayInvisibility);
                }
            }
            else
            {
                Cancel();
            }
        }

        private void ApplyInvisibilityMaterial(RenderView renderView)
        {
            foreach (MaterialsSnapshot snapshot in renderView.InitialMaterials)
            {
                int length = snapshot.Materials.Length;
                var materials = new Material[length];
                for (int i = 0; i < length; i++)
                {
                    materials[i] = spec.InvisibilityMaterial;
                }

                snapshot.Renderer.sharedMaterials = materials;
            }
        }

        private void RevertMaterials()
        {
            Character.CharacterCommon.TrooperAvatar.RenderView.ApplyDefaultMaterials();
            Character.CharacterView.WeaponView.RenderView.ApplyDefaultMaterials();
        }

        private void SetLayer(int layer)
        {
            var children = Character.transform.GetComponentsInChildren<Transform>(true);
            foreach (Transform trans in children)
            {
                if (trans.gameObject.layer != spec.AimAssistLayer)
                {
                    trans.gameObject.layer = layer;
                }
            }

            if (Character is LocalCharacter)
            {
                var kinematicCharacterMotor = ((LocalCharacter)Character).GetComponent<KinematicCharacterMotor>();

                kinematicCharacterMotor.CollidableLayers = 0;
                for (int i = 0; i < 32; i++)
                {
                    if (!Physics.GetIgnoreLayerCollision(Character.gameObject.layer, i))
                    {
                        kinematicCharacterMotor.CollidableLayers |= (1 << i);
                    }
                }
            }
        }
    }
}