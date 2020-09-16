using UnityEngine;
using UnityEngine.EventSystems;

namespace XenStudio.UI
{
    public class SetTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public string Tip;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (string.IsNullOrEmpty(Tip))
            {
                Debug.LogWarning("Tooltip message empty.");
            }
            else
            {
                TipDisplayer.Instance.SetTip(Tip);
            }
        }


        public void OnPointerExit(PointerEventData eventData)
        {
            TipDisplayer.Instance.SetTip(string.Empty);
        }
    }

}