using System.Collections.Generic;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.Purchasing.Products;
using Heyworks.PocketShooter.UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI
{
    public class ShopCategoryPresenter : IDisposablePresenter
    {
        private ShopCategoryButton categoryButton;
        private List<ShopCard> cards = new List<ShopCard>();

        public ScrollRect Scroll { get; private set; }

        public ShopCategory CategoryType { get; }

        public IReadOnlyList<ShopCard> Cards => cards;

        public ShopCategoryPresenter(
            ShopCategory category,
            ShopProduct[] products,
            Player player,
            ShopCategoryButton categoryButton,
            ScrollRect scroll,
            ShopCard shopCardPrefab)
        {
            this.CategoryType = category;
            this.categoryButton = categoryButton;
            this.Scroll = scroll;
          
            categoryButton.Setup(category);

            foreach (var item in products)
            {
                ShopCard card = Object.Instantiate(shopCardPrefab, scroll.content);
                card.Setup(item, player);
                cards.Add(card);
            }
        }

        public void Dispose()
        {
            Object.Destroy(Scroll.gameObject);
        }

        public void SetActive(bool active)
        {
            Scroll.gameObject.SetActive(active);
            categoryButton.SetActive(active);
        }
    }
}