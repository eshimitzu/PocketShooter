using System;
using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.Purchasing.Core;
using Heyworks.PocketShooter.Purchasing.Products;
using Heyworks.PocketShooter.UI.Core;
using Heyworks.PocketShooter.UI.Localization;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Heyworks.PocketShooter.Modules.Analytics;
using Microsoft.Extensions.Logging;

namespace Heyworks.PocketShooter.UI.Popups
{
    public class OfferPopupPresenter : IDisposablePresenter
    {
        private IConfigurationsProvider configurationsProvider;
        private Shop shop;
        private OfferItemsFactory offerItemsFactory;
        private OfferPopupData offerPopupData;
        private OfferPopup offerPopup;

        public OfferPopupPresenter(IConfigurationsProvider configurationsProvider, Shop shop, OfferItemsFactory offerItemsFactory)
        {
            this.configurationsProvider = configurationsProvider;
            this.shop = shop;
            this.offerItemsFactory = offerItemsFactory;
        }

        public void Dispose()
        {
        }

        public void LobbyScreenLoadedAfterBattle()
        {
            ShopProduct[] shopProducts = shop.GetVisibleShopProductsWithCategory(ShopCategory.Offers);

            if (shopProducts.Length > 0)
            {
                IReadOnlyList<OfferPopupData> allOfferPopupDatas = configurationsProvider.OfferPopupConfiguration.GetOfferPopups();
                List<OfferPopupData> readyToShowingOfferPopupDatas = new List<OfferPopupData>();

                for (int i = 0; i < allOfferPopupDatas.Count; i++)
                {
                    if (shopProducts.FirstOrDefault(item => item.Id == allOfferPopupDatas[i].OfferProductId) != null)
                    {
                        readyToShowingOfferPopupDatas.Add(allOfferPopupDatas[i]);
                    }
                }

                if (readyToShowingOfferPopupDatas.Count > 0)
                {
                    OfferPopupData popupData = readyToShowingOfferPopupDatas[UnityEngine.Random.Range(0, readyToShowingOfferPopupDatas.Count)];
                    ShopProduct offer = shopProducts.First(item => item.Id == popupData.OfferProductId);

                    if (UnityEngine.Random.Range(0f, 1f) <= popupData.AppearanceChance)
                    {
                        offerPopupData = popupData;
                        OfferType offerType = ShopProdutIDToType(offer.Id);

                        offerPopup = ScreensController.Instance.ShowPopup<OfferPopup>();

                        offerPopup.AcceptButtonOnClick += OfferPopup_AcceptButtonOnClick;
                        offerPopup.CloseButtonOnClick += OfferPopup_CloseButtonOnClick;

                        offerPopup.TitleLabel.SetLocalizedText(LocKeys.Offers.GetOfferNameKey(offerType));
                        offerPopup.DescriptionLabel.SetLocalizedText(LocKeys.Offers.GetOfferDescriptionKey(offerType));
                        offerPopup.Banner.sprite = offerItemsFactory.BannerSpriteForOffer(offerType);

                        bool isTrooperOffer = false;
                        TrooperClass trooperClass = default;
                        ArmorName armorName = default;

                        switch (offerType)
                        {
                            case OfferType.Norris:
                                trooperClass = TrooperClass.Norris;
                                isTrooperOffer = true;
                                break;
                            case OfferType.Neo:
                                trooperClass = TrooperClass.Neo;
                                isTrooperOffer = true;
                                break;
                            case OfferType.Statham:
                                trooperClass = TrooperClass.Statham;
                                isTrooperOffer = true;
                                break;
                            case OfferType.Rock:
                                trooperClass = TrooperClass.Rock;
                                isTrooperOffer = true;
                                break;
                            case OfferType.Armor2:
                                armorName = ArmorName.Armor2;
                                isTrooperOffer = false;
                                break;
                            case OfferType.Armor3:
                                armorName = ArmorName.Armor3;
                                isTrooperOffer = false;
                                break;
                            case OfferType.Armor4:
                                armorName = ArmorName.Armor4;
                                isTrooperOffer = false;
                                break;
                            case OfferType.Armor5:
                                armorName = ArmorName.Armor5;
                                isTrooperOffer = false;
                                break;
                        }

                        if (isTrooperOffer)
                        {
                            offerPopup.TrooperNameLabel.SetLocalizedText(LocKeys.GetTooperNameKey(trooperClass));
                        }
                        else
                        {
                            offerPopup.TrooperNameLabel.SetLocalizedText(LocKeys.GetArmorNameKey(armorName));
                        }

                        if (popupData.Discount > 0)
                        {
                            offerPopup.DiscountView.SetActive(true);
                            offerPopup.DiscountLabel.text = "-" + popupData.Discount.ToString() + "%";
                        }
                        else
                        {
                            offerPopup.DiscountView.SetActive(false);
                        }

                        offerPopup.TrooperNameLabel.text = offerPopup.TrooperNameLabel.text.ToUpper();
                    }
                }
            }
        }

        private OfferType ShopProdutIDToType(string id)
        {
            switch (id)
            {
                case "product_t2_starter_hero_20":
                case "product_test_t2_starter_hero_20":
                    return OfferType.Norris;
                case "product_t3_starter_hero_50":
                case "product_test_t3_starter_hero_50":
                    return OfferType.Neo;
                case "product_t4_starter_hero_100":
                case "product_test_t4_starter_hero_100":
                    return OfferType.Statham;
                case "product_t5_starter_hero_200":
                case "product_test_t5_starter_hero_200":
                    return OfferType.Rock;
                case "product_t2_starter_armr_15":
                case "product_test_t2_starter_armr_15":
                    return OfferType.Armor2;
                case "product_t3_starter_armr_37":
                case "product_test_t3_starter_armr_37":
                    return OfferType.Armor3;
                case "product_t4_starter_armr_75":
                case "product_test_t4_starter_armr_75":
                    return OfferType.Armor4;
                case "product_t5_starter_armr_150":
                case "product_test_t5_starter_armr_150":
                    return OfferType.Armor5;
                default:
                    GameLog.Log.LogError("No offer type for" + id);
                    return OfferType.None;
            }
        }

        private void OfferPopup_AcceptButtonOnClick()
        {
            AnalyticsManager.Instance.SendOfferPopupActions(offerPopupData.OfferProductId, AnalyticsManager.OfferAction.ToShop);
            var screen = ScreensController.Instance.ShowScreen<ShopScreen>();
            screen.Setup(ShopCategory.Offers);

            OfferPopup_CloseButtonOnClick();
        }

        private void OfferPopup_CloseButtonOnClick()
        {
            AnalyticsManager.Instance.SendOfferPopupActions(offerPopupData.OfferProductId, AnalyticsManager.OfferAction.Close);

            offerPopup.AcceptButtonOnClick -= OfferPopup_AcceptButtonOnClick;
            offerPopup.CloseButtonOnClick -= OfferPopup_CloseButtonOnClick;

            offerPopup.Hide();
            offerPopup = null;
        }
    }
}