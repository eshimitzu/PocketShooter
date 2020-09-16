using System.Collections.Generic;
using Heyworks.PocketShooter.Core;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Purchasing.Core;
using Heyworks.PocketShooter.Purchasing.Products;
using Heyworks.PocketShooter.UI.Core;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Heyworks.PocketShooter.UI
{
    public class ShopScreen : BaseScreen
    {
        [SerializeField]
        private LobbyProfileBar profileBar;

        [SerializeField]
        private ShopCard shopCardPrefab;

        [SerializeField]
        private ShopCategoryButton shopCategoryButtonPrefab;

        [SerializeField]
        private RectTransform categoriesRoot;

        [SerializeField]
        private RectTransform productsScrollRoot;

        [SerializeField]
        private ScrollRect scrollPrefab;

        [SerializeField]
        private Text noOffersLabel;

        [Inject]
        private ScreensController screensController;

        [Inject]
        private Main main;

        private List<ShopCategoryPresenter> categoryPresenters = new List<ShopCategoryPresenter>();
        private ShopCategory lastSelectedCategory;

        private void Start()
        {
            profileBar.OnStoreClick += LobbyProfileBar_OnStoreClick;
            profileBar.OnBackClick += () =>
            {
                var screen = screensController.ShowScreen<LobbyScreen>();
                screen.Setup(main);
            };

            main.MetaGame.Shop.ProductsListUpdated += Shop_ProductsListUpdated;
        }

        /// <summary>
        /// Called when screen is destroyed.
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();

            main.MetaGame.Shop.ProductsListUpdated -= Shop_ProductsListUpdated;
        }

        public void Setup(ShopCategory categoryType)
        {
            var lobbyPresenter = new LobbyScreenPresenter(profileBar, main);
            AddDisposablePresenter(lobbyPresenter);

            var fightButtonPresenter = new LobbyFightButtonPresenter(profileBar, main);
            AddDisposablePresenter(fightButtonPresenter);

            UpdateProducts(main.MetaGame.Shop, categoryType);
        }

        private void UpdateProducts(Shop shop, ShopCategory categoryToSelect)
        {
            foreach (Transform child in categoriesRoot)
            {
                Destroy(child.gameObject);
            }

            List<ShopCategoryPresenter> newCategoryPresenters = new List<ShopCategoryPresenter>();

            foreach (ShopCategory category in GetAllShopCategories())
            {
                ShopCategoryButton categoryButton = Instantiate(shopCategoryButtonPrefab, categoriesRoot);
                ScrollRect scrollRect = Instantiate(scrollPrefab, productsScrollRoot);

                var shopProducts = GetShopProducts(shop, category);

                var categoryPresenter = new ShopCategoryPresenter(
                    category,
                    shopProducts,
                    main.MetaGame.Player,
                    categoryButton,
                    scrollRect,
                    shopCardPrefab);
                categoryButton.OnClick += () => { SelectCategory(categoryPresenter.CategoryType); };
                newCategoryPresenters.Add(categoryPresenter);

                ShopCategoryPresenter oldCategoryPresenter = categoryPresenters.Find(_ => _.CategoryType == category);

                if (oldCategoryPresenter != null)
                {
                    Debug.Log(oldCategoryPresenter.Scroll.content.anchoredPosition);
                    scrollRect.content.anchoredPosition = oldCategoryPresenter.Scroll.content.anchoredPosition;
                }
                else
                {
                    RectTransform content = scrollRect.content;
                    content.anchoredPosition = new Vector2(
                        50000,
                        content.anchoredPosition.y); // or wait till layout will update and calculate offset
                }
            }

            foreach (ShopCategoryPresenter presenter in categoryPresenters)
            {
                presenter.Dispose();
            }

            categoryPresenters.Clear();
            categoryPresenters = newCategoryPresenters;

            SelectCategory(categoryToSelect);
        }

        private void Shop_ProductsListUpdated()
        {
            UpdateProducts(main.MetaGame.Shop, lastSelectedCategory);
        }

        private static ShopProduct[] GetShopProducts(Shop shop, ShopCategory category)
        {
            return shop.GetVisibleShopProductsWithCategory(category);
        }

        private ShopCategory[] GetAllShopCategories()
        {
            return new[] { ShopCategory.Offers, ShopCategory.Hard, ShopCategory.Soft, ShopCategory.Consumables };
        }

        private void LobbyProfileBar_OnStoreClick(ShopCategory category)
        {
            UpdateProducts(main.MetaGame.Shop, category);
        }

        private void SelectCategory(ShopCategory category)
        {
            lastSelectedCategory = category;
            foreach (ShopCategoryPresenter shopCategoryPresenter in categoryPresenters)
            {
                shopCategoryPresenter.SetActive(shopCategoryPresenter.CategoryType == category);
            }

            ShopCategoryPresenter selected = categoryPresenters.Find(f => f.CategoryType == category);
            noOffersLabel.gameObject.SetActive(selected.Cards.Count == 0);
        }
    }
}