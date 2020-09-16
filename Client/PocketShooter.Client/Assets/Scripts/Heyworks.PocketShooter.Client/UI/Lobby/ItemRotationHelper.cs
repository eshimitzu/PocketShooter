using UnityEngine;
using UnityEngine.EventSystems;

namespace Heyworks.PocketShooter.UI
{
    public class ItemRotationHelper : MonoBehaviour, IDragHandler
    {
        public void OnDrag(PointerEventData eventData)
        {
            foreach (Transform child in transform)
            {
                child.localRotation *= Quaternion.Euler(0, -eventData.delta.x, 0);
            }
        }
    }
}
