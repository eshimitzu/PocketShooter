using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Heyworks.PocketShooter.Utils;
using UnityEngine;

namespace Heyworks.PocketShooter.UI.TrooperSelection
{
    /// <summary>
    /// Represents TrooperSelectionView.
    /// </summary>
    public class TrooperSelectionView : MonoBehaviour
    {
        [SerializeField]
        private Transform itemsAnchor = null;
        [SerializeField]
        private TrooperSelectionItem itemPrefab = null;

        private List<TrooperSelectionItem> itemsCollection;

        /// <summary>
        /// Represents ItemSelected.
        /// </summary>
        public event Action<TrooperSelectionParameters> ItemSelected;

        private void Awake()
        {
            itemsCollection = new List<TrooperSelectionItem>();
        }

        /// <summary>
        /// Show.
        /// </summary>
        /// <param name="parameters">parameters.</param>
        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed.")]
        public void Show(TrooperSelectionParameters[] parameters)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                var item = Instantiate(itemPrefab, itemsAnchor, true);
                item.transform.Reset(true);
                itemsCollection.Add(item);

                item.Setup(parameters[i]);
                item.Selected += Item_Selected;
            }
        }

        /// <summary>
        /// Hide.
        /// </summary>
        public void Hide()
        {
            for (int i = 0; i < itemsCollection.Count; i++)
            {
                itemsCollection[i].Selected -= Item_Selected;
                Destroy(itemsCollection[i].gameObject);
            }

            itemsCollection.Clear();
        }

        private void Item_Selected(TrooperSelectionParameters parameters)
        {
            ItemSelected?.Invoke(parameters);
        }
    }
}