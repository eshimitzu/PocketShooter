using System;
using System.Diagnostics.CodeAnalysis;
using Heyworks.PocketShooter.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.TrooperSelection
{
    /// <summary>
    /// Represents trooperSelectionItem.
    /// </summary>
    public class TrooperSelectionItem : MonoBehaviour
    {
        [SerializeField]
        private Transform characterAnchor = null;
        [SerializeField]
        private Button interactionButton = null;

        private TrooperSelectionParameters parameters;

        /// <summary>
        /// Selected event.
        /// </summary>
        public event Action<TrooperSelectionParameters> Selected;

        private void Start()
        {
            interactionButton.onClick.AddListener(InteractionButton_Click);
        }

        /// <summary>
        /// Setup.
        /// </summary>
        /// <param name="parameters">parameters.</param>
        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed.")]
        public void Setup(TrooperSelectionParameters parameters)
        {
            this.parameters = parameters;

            SetupCharacterGameObject(parameters.TrooperGameObject);
        }

        private void SetupCharacterGameObject(GameObject characterGameObject)
        {
            characterGameObject.transform.SetParent(characterAnchor);
            characterGameObject.transform.Reset(true);
            characterGameObject.RunOnChildrenRecursive(child => child.layer = LayerMask.NameToLayer("UI"));
        }

        private void InteractionButton_Click()
        {
            Selected?.Invoke(parameters);
        }
    }
}