using Heyworks.PocketShooter.Character;
using Heyworks.PocketShooter.Skills;
using Heyworks.PocketShooter.UI.Core;
using Heyworks.PocketShooter.UI.Localization;
using SRF;
using UnityEngine;

namespace Heyworks.PocketShooter.UI
{
    public class BaseSelectionPresenter : IDisposablePresenter
    {
        private readonly BaseSelectionView lobbySelectionView;
        private readonly TrooperCreator trooperCreator;
        private GameObject trooper;
        private IconsFactory itemsFactory;

        private IRosterItem SelectedItem => CardPresenter.Item;

        protected ILobbyCardPresenter CardPresenter { get; private set; }

        public BaseSelectionPresenter(
            BaseSelectionView lobbySelectionView,
            TrooperCreator trooperCreator,
            IconsFactory itemsFactory)
        {
            this.trooperCreator = trooperCreator;
            this.lobbySelectionView = lobbySelectionView;
            this.itemsFactory = itemsFactory;

            lobbySelectionView.ItemTooltipView.gameObject.SetActive(false);

            lobbySelectionView.SkillButton1.OnClick += SkillButton1_OnClick;
            lobbySelectionView.SkillButton2.OnClick += SkillButton2_OnClick;
            lobbySelectionView.SkillButton3.OnClick += SkillButton3_OnClick;
        }

        public virtual void Dispose()
        {
            lobbySelectionView.SkillButton1.OnClick -= SkillButton1_OnClick;
            lobbySelectionView.SkillButton2.OnClick -= SkillButton2_OnClick;
            lobbySelectionView.SkillButton3.OnClick -= SkillButton3_OnClick;
        }

        public void Update()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    ScreenClicked();
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                ScreenClicked();
            }
        }

        private void ScreenClicked()
        {
            lobbySelectionView.ItemTooltipView.gameObject.SetActive(false);
        }

        public virtual void ShowCard(ILobbyCardPresenter presenter, bool inBattle)
        {
            // TODO: v.shimkovich PSH-844
            if (presenter == null)
            {
                return;
            }

            CardPresenter = presenter;

            DestroyPreview();

            bool isTrooper = SelectedItem is ITrooperItem;
            lobbySelectionView.SkillsBar.gameObject.SetActive(isTrooper);
            lobbySelectionView.ItemsBar.gameObject.SetActive(isTrooper);
            lobbySelectionView.ItemPreview.enabled = !isTrooper;

            if (SelectedItem is ITrooperItem trooperItem)
            {
                UpdateSkills(trooperItem);

                var battleTrooper = trooperItem as IBattleTrooperItem;
                SpawnTrooper(trooperItem.Class, battleTrooper?.WeaponName);

                if (battleTrooper != null)
                {
                    lobbySelectionView.WeaponButton.Setup(
                        itemsFactory.SmallSpriteForWeaponItem(battleTrooper.WeaponName),
                        battleTrooper.WeaponPower.ToString(),
                        !inBattle);
                    lobbySelectionView.HelmetButton.Setup(
                        itemsFactory.SmallSpriteForHelmetItem(battleTrooper.HelmetName),
                        battleTrooper.HelmetPower.ToString(),
                        !inBattle);
                    lobbySelectionView.ArmorButton.Setup(
                        itemsFactory.SmallSpriteForArmorItem(battleTrooper.ArmorName),
                        battleTrooper.ArmorPower.ToString(),
                        !inBattle);
                }
            }
            else
            {
                lobbySelectionView.ItemPreview.sprite = itemsFactory.SpriteForItem(SelectedItem);
            }

            lobbySelectionView.MainCard.Setup(SelectedItem);
            lobbySelectionView.ItemName?.SetLocalizedText(SelectedItem.ItemName);
        }

        private void UpdateSkills(ITrooperItem trooper)
        {
            SkillControllerFactory skillFactory = lobbySelectionView.SkillFactory;
            lobbySelectionView.SkillButton1.Icon.sprite = skillFactory.GetSkillSpec(trooper.Skill1)?.Icon;
            lobbySelectionView.SkillButton2.Icon.sprite = skillFactory.GetSkillSpec(trooper.Skill2)?.Icon;
            lobbySelectionView.SkillButton3.Icon.sprite = skillFactory.GetSkillSpec(trooper.Skill3)?.Icon;
        }

        private void DestroyPreview()
        {
            if (trooper)
            {
                Object.Destroy(trooper);
                trooper = null;
            }
        }

        private void SpawnTrooper(TrooperClass trooperClass, WeaponName? weaponName)
        {
            trooper = CreateTrooper(
                trooperClass,
                weaponName,
                lobbySelectionView.ItemPreviewRoot);
        }

        private GameObject CreateTrooper(
            TrooperClass trooperClass,
            WeaponName? weaponIdentifier,
            RectTransform itemRoot)
        {
            GameObject go = trooperCreator.CreateDummyTrooper(trooperClass, weaponIdentifier);
            go.transform.parent = itemRoot;
            go.transform.localPosition = Vector3.zero - itemRoot.rect.height * 0.85f * Vector3.up;
            go.transform.localScale = itemRoot.rect.height * 1.4f * Vector3.one;
            go.transform.rotation = Quaternion.Euler(0, 180, 0);
            go.SetLayerRecursive(itemRoot.gameObject.layer);

            return go;
        }

        private void ShowTooltip(LobbySkillButton skillButton, SkillName skillName)
        {
            lobbySelectionView.ItemTooltipView.gameObject.SetActive(true);

            Vector3 toolTipPosition = skillButton.transform.position;

            Rect skillButtonRect = skillButton.GetComponent<RectTransform>().rect;

            toolTipPosition.x = toolTipPosition.x - skillButtonRect.size.x * 0.75f;
            toolTipPosition.y = toolTipPosition.y + skillButtonRect.size.y * 0.5f;
            toolTipPosition.z = 0f;

            lobbySelectionView.ItemTooltipView.transform.position = toolTipPosition;

            lobbySelectionView.ItemTooltipView.TitleLabel.text = LocKeys.GetLocKeyForSkillName(skillName).Localized().ToUpper();
            lobbySelectionView.ItemTooltipView.DescriptionLabel.SetLocalizedText(LocKeys.GetLocKeyForDescriptionSkillName(skillName));
        }

        private void SkillButton1_OnClick()
        {
            if (SelectedItem is ITrooperItem trooperItem)
            {
                ShowTooltip(lobbySelectionView.SkillButton1, trooperItem.Skill1);
            }
        }

        private void SkillButton2_OnClick()
        {
            if (SelectedItem is ITrooperItem trooperItem)
            {
                ShowTooltip(lobbySelectionView.SkillButton2, trooperItem.Skill2);
            }
        }

        private void SkillButton3_OnClick()
        {
            if (SelectedItem is ITrooperItem trooperItem)
            {
                ShowTooltip(lobbySelectionView.SkillButton3, trooperItem.Skill3);
            }
        }
    }
}