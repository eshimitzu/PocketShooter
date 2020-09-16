using System;
using UnityEngine.EventSystems;

namespace Heyworks.PocketShooter.UI.Common
{
    public class UIBehaviourEvents : UIBehaviour
    {
        public event Action OnRectTransformDimensionsChanged;

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            OnRectTransformDimensionsChanged?.Invoke();
        }
    }
}