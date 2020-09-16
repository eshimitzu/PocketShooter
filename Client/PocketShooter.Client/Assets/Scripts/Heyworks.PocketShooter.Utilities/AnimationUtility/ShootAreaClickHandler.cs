using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Heyworks.PocketShooter.Utilities.AnimationUtility
{
    public class ShootAreaClickHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public Action OnShootAreaClick;

        private bool isPointerDown = false;

        public void OnPointerDown(PointerEventData eventData)
        {
            isPointerDown = true;
            OnShootAreaClick?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPointerDown = false;
        }

        private void Update()
        {
            if (isPointerDown)
            {
                OnShootAreaClick?.Invoke();
            }
        }
    }
}
