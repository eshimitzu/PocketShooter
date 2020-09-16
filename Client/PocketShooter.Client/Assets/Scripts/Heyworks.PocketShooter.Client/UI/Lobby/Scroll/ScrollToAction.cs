using System;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI
{
    public class ScrollToAction : IScrollAction
    {
        private const float MaxScrollVelocity = 5000f;
        private const float AccelerateTime = 0.2f;
        private const float MaxDecelerateDistance = 500f;

        private RosterScroll roster;
        private ScrollRect scroll;
        private Action onFinished;

        private float start;
        private float end;
        private float length;
        private float dir;

        private float time;
        private float time2;
        private float velocity;

        private float decelerateStartPos;
        private float decelerateDistance;

        private bool finished;

        public ScrollToAction(RosterScroll roster, ScrollRect scroll, int index, Action onFinished)
        {
            this.roster = roster;
            this.scroll = scroll;
            this.onFinished = onFinished;

            velocity = scroll.velocity.x;
            start = scroll.content.anchoredPosition.x;
            end = roster.GetIndexPosition(index);
            length = Mathf.Abs(end - start);
            dir = end > start ? 1 : -1;
            decelerateDistance = Mathf.Clamp(length * 0.5f, 0, MaxDecelerateDistance);

            time = Mathf.Clamp(velocity * dir / MaxScrollVelocity, 0, 1);

            scroll.StopMovement();
        }

        public void Update()
        {
            if (!finished)
            {
                Vector3 position = scroll.content.anchoredPosition;
                float pos = position.x;

                if (Mathf.Abs(start - pos) < length * 0.5f)
                {
                    time += Time.deltaTime;
                    float easeValue = EaseInQuad(0, 1, time, AccelerateTime);
                    velocity = Mathf.Lerp(0, MaxScrollVelocity, easeValue) * dir;
                }
                else if (Mathf.Abs(end - pos) < decelerateDistance)
                {
                    velocity = 0;

                    if (time2 < 0.01f)
                    {
                        decelerateStartPos = pos;
                    }

                    time2 += Time.deltaTime;
                    float easeValue = EaseOutQuad(0, 1, time2, AccelerateTime);
                    pos = Mathf.Lerp(decelerateStartPos, end, easeValue);
                    scroll.content.anchoredPosition = new Vector2(pos, position.y);
                }

                scroll.velocity = new Vector2(velocity, 0);

                if (Math.Abs(pos - end) < 10f)
                {
                    finished = true;
                    onFinished?.Invoke();
                }
            }
        }

        static float EaseInQuad(float start, float distance, float elapsedTime, float duration)
        {
            elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
            return distance * elapsedTime * elapsedTime + start;
        }

        static float EaseOutQuad(float start, float distance, float elapsedTime, float duration)
        {
            elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
            return -distance * elapsedTime * (elapsedTime - 2) + start;
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