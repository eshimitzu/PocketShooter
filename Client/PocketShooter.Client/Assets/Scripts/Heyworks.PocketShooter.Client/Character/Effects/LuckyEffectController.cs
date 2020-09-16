using Heyworks.PocketShooter.Audio;
using Heyworks.PocketShooter.Client;
using Heyworks.PocketShooter.Networking.Actors;
using UniRx;
using UnityEngine;

namespace Heyworks.PocketShooter.Character.Effects
{
    public class LuckyEffectController : EffectController
    {
        [SerializeField]
        private Material luckyHeadMaterial;

        [SerializeField]
        private Material luckyBodyMaterial;

        private bool isLucky;
        private Material[] materials;

        public override void Setup(NetworkCharacter character)
        {
            base.Setup(character);

            // create materials copy for each trooper
            materials = new[] { Instantiate(luckyBodyMaterial), Instantiate(luckyHeadMaterial) };

            character.ModelEvents.LuckyChanged.Subscribe(UpdateEffect).AddTo(this);
        }

        public override void Stop()
        {
            if (isLucky)
            {
                isLucky = false;

                Character.CharacterCommon.TrooperAvatar.RenderView.ApplyDefaultMaterials();
                Character.CharacterCommon.AudioController.PostEvent(AudioKeys.Event.PlayLucky);
            }
        }

        private void UpdateEffect(bool lucky)
        {
            if (lucky)
            {
                isLucky = true;

                // hardcoded only for rock
                TrooperAvatar avatar = Character.CharacterCommon.TrooperAvatar;
                MaterialsSnapshot trooper = avatar.RenderView.CurrentMaterials[0];

                if (Character is RemoteCharacter)
                {
                    for (int i = 0; i < trooper.Materials.Length; i++)
                    {
                        materials[i].SetRimColor(trooper.Materials[i].GetRimColor());
                        materials[i].SetOutlineColor(trooper.Materials[i].GetOutlineColor());
                        materials[i].SetRimPower(trooper.Materials[i].GetRimPower());
                        materials[i].SetOutline(trooper.Materials[i].GetOutline());
                    }
                }

                trooper.Renderer.sharedMaterials = materials;

                Character.CharacterCommon.AudioController.PostEvent(AudioKeys.Event.PlayLucky);
            }
            else
            {
                Stop();
            }
        }

        private void OnDestroy()
        {
            CleanMaterials();
        }

        private void CleanMaterials()
        {
            if (materials == null)
            {
                return;
            }

            foreach (Material material in materials)
            {
                if (material)
                {
                    Destroy(material);
                }
            }

            materials = null;
        }
    }
}