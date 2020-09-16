using System;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI
{
    public class ScrollSnapAction : IScrollAction
    {
        private ScrollRect scroll;
        private float snapDuration;
        private Action onFinished;

        private float easeTime;
        private float easeStart;
        private float easeEnd;

        private bool finished;

        public ScrollSnapAction(RosterScroll roster, ScrollRect scroll, float snapDuration, Action onFinished)
        {
            this.scroll = scroll;
            this.snapDuration = snapDuration;
            this.onFinished = onFinished;

            easeStart = scroll.content.anchoredPosition.x;
            easeEnd = roster.GetIndexPosition(roster.CurrentIndex);
        }

        public void Update()
        {
            if (!finished)
            {
                Vector3 position = scroll.content.anchoredPosition;
                easeTime += Time.deltaTime;
                float normalizedTime = Mathf.Clamp(easeTime / snapDuration, 0, 1);
                float easeValue = EaseInOutCubic(0, 1, normalizedTime);
                float pos = Mathf.Lerp(easeStart, easeEnd, easeValue);
                scroll.content.anchoredPosition = new Vector2(pos, position.y);

                if (Math.Abs(normalizedTime - 1) < 0.01f)
                {
                    finished = true;
                    onFinished?.Invoke();
                }
            }
        }

        private float EaseInOutCubic(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if (value < 1)
            {
                return end * 0.5f * value * value * value + start;
            }

            value -= 2;
            return end * 0.5f * (value * value * value + 2) + start;
        }
    }
}
