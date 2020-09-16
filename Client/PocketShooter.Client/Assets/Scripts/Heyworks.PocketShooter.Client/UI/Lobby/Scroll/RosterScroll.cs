using System;
using Heyworks.PocketShooter.Utils.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI
{
    public class RosterScroll : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField]
        private float snapMinVelocity = 200;

        [SerializeField]
        private float snapDuration = 0.1f;

        public event Action<int> OnScrollIndexUpdated;

        private int prevIndex = -1;
        private bool isDrag;
        private ScrollRect scroll;
        private RectTransform content;
        private HorizontalLayoutGroup contentLayout;
        private IScrollAction currentAction;

        public int CurrentIndex => GetScrollIndex();

        public HorizontalLayoutGroup ContentLayout => contentLayout;

        public float RosterCardWidth => 206;

        public float RosterCardDelta => contentLayout.spacing;

        public void OnBeginDrag(PointerEventData eventData)
        {
            isDrag = true;
            currentAction = null;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isDrag = false;
        }

        public void ScrollToIndex(int index, bool animated = true)
        {
            if (animated)
            {
                currentAction = new ScrollToAction(
                    this,
                    scroll,
                    index,
                    () => { currentAction = null; });
            }
            else
            {
                float pos = GetIndexPosition(index);
                scroll.content.anchoredPosition = new Vector2(pos, scroll.content.anchoredPosition.y);
            }
        }

        public float GetIndexPosition(int index)
        {
            float leftAnchor = scroll.viewport.rect.width * 0.5f + contentLayout.padding.left;

            float pos = (RosterCardWidth + RosterCardDelta) * index;
            float anchorPos = -pos - leftAnchor;
            return anchorPos;
        }

        private void Awake()
        {
            scroll = GetComponent<ScrollRect>();
            content = scroll.content;
            contentLayout = content.GetComponent<HorizontalLayoutGroup>();
        }

        private void LateUpdate()
        {
            UpdateScrollPosition();

            int index = CurrentIndex;
            if (prevIndex != index)
            {
                OnScrollIndexUpdated?.Invoke(index);
                prevIndex = index;
            }
        }

        private void UpdateScrollPosition()
        {
            float speed = scroll.velocity.magnitude;
            if (!isDrag && currentAction == null && speed < snapMinVelocity)
            {
                scroll.StopMovement();

                currentAction = new ScrollSnapAction(
                    this,
                    scroll,
                    snapDuration,
                    () => { currentAction = null; });
            }

            currentAction?.Update();
        }

        private int GetScrollIndex()
        {
            float leftAnchor = scroll.viewport.rect.width * 0.5f + contentLayout.padding.left;
            float pos = -(scroll.content.anchoredPosition.x + leftAnchor);
            float cardCenter = pos + RosterCardWidth * 0.5f;

            int index = (int)(cardCenter / (RosterCardWidth + RosterCardDelta));

            // subtract overflow cell
            return Mathf.Clamp(index, 0, scroll.content.childCount - 1);
        }
    }
}