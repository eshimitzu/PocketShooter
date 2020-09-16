using Heyworks.PocketShooter.Character;
using Heyworks.PocketShooter.Meta.Communication;
using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.UI.Common;
using SRF;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.EndBattle
{
    public class ResultTrooperCell : MonoBehaviour
    {
        [SerializeField]
        private Image titleBackground;

        [SerializeField]
        private Image mvpIcon;

        [SerializeField]
        private Text titleLabel;

        [SerializeField]
        private Text killsValueLabel;

        [SerializeField]
        private Text deathsValueLabel;

        [SerializeField]
        private Text goldRewardLabel;

        [SerializeField]
        private Text cacheRewardLabel;

        [SerializeField]
        private Text expRewardLabel;

        [SerializeField]
        private RectTransform rewardsRoot;

        [SerializeField]
        private RectTransform trooperRoot;

        [SerializeField]
        private RectTransform shadow;

        [SerializeField]
        private UIBehaviourEvents trooperRootEvents;

        private GameObject trooper;
        private int index;

        private float masterScale = 0.7f;
        private float[] trooperScales = { 0.9f, 0.8f, 0.8f, 0.7f, 0.7f };


        public void Setup(TrooperCreator trooperCreator, PlayerMatchStatsData player, int index)
        {
            this.index = index;

            titleLabel.text = player.Nickname;
            killsValueLabel.text = player.Kills.ToString();
            deathsValueLabel.text = player.Deaths.ToString();
            mvpIcon.enabled = player.IsMVP;

            trooper = CreateTrooper(trooperCreator, player.TrooperClass, player.CurrentWeapon, trooperRoot);
        }

        public void Setup(PlayerReward reward)
        {
            rewardsRoot.gameObject.SetActive(true);
            goldRewardLabel.text = $"+{reward.Gold}";
            cacheRewardLabel.text = $"+{reward.Cash}";
            expRewardLabel.text = $"+{reward.Experience}";
        }

        private void OnEnable()
        {
            trooperRootEvents.OnRectTransformDimensionsChanged += TrooperRootOnRectChanged;
        }

        private void OnDisable()
        {
            trooperRootEvents.OnRectTransformDimensionsChanged -= TrooperRootOnRectChanged;
        }

        private void TrooperRootOnRectChanged()
        {
            if (trooper)
            {
                UpdateTrooper(trooper);
            }
        }

        private GameObject CreateTrooper(
            TrooperCreator trooperCreator,
            TrooperClass trooperClass,
            WeaponName weaponName,
            RectTransform itemRoot)
        {
            GameObject go = trooperCreator.CreateDummyTrooper(trooperClass, weaponName);
            go.transform.parent = itemRoot;
            go.transform.rotation = Quaternion.Euler(0, 180, 0);
            go.SetLayerRecursive(itemRoot.gameObject.layer);
            UpdateTrooper(go);

            return go;
        }

        private void UpdateTrooper(GameObject trooper)
        {
            float scale = trooperScales[index] * masterScale;
            float height = trooperRoot.rect.height * scale;
            trooper.transform.localScale = height * Vector3.one;
            trooper.transform.localPosition = Vector3.zero - trooperRoot.rect.height * 0.45f * Vector3.up;
            shadow.localScale = scale * Vector3.one;
            shadow.transform.position = trooper.transform.position;
        }
    }
}