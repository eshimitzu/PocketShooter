using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Heyworks.PocketShooter.UI.Animation
{
    public abstract class Tweener : MonoBehaviour, ITweener
    {
        public const float TweenFactorEpsilon = 0.001f;

        #region Variables

        public Method Method = Method.Linear;
        public Style Style = Style.Once;

        public AnimationCurve AnimationCurve = new AnimationCurve(
            new Keyframe(0f, 0f, 0f, 1f),
            new Keyframe(1f, 1f, 1f, 0f));

        public bool UseCurve;

        public bool IgnoreTimeScale = true;

        public float Duration = 1f;

        public float ScaleDuration = 1f;

        public bool SteeperCurves = true;

        public InvokeData InvokeWhenFinished = new InvokeData();

        public bool ShouldSetBeginStateAtStart = false;

        [SerializeField]
        private bool notUsedForScreenAnimatingCondition;

        private List<KeyValuePair<string, TimedCallback>> timedCallbacks =
            new List<KeyValuePair<string, TimedCallback>>();

        private float cachedDuration = -1f;
        private float cachedScaleDuration = -1f;
        private float amountPerDelta = 1f;
        private float factor;
        private TweenCallback onFinished;

        private Transform cachedTransform;
        private bool started;
        private float startTime;
        private float delay;
        private bool isReversed = false;

        private WaitAction waitAndDoCallback;

        public float AmountPerDelta
        {
            get
            {
                if ((cachedDuration != Duration) || (cachedScaleDuration != ScaleDuration))
                {
                    cachedDuration = Duration;
                    cachedScaleDuration = ScaleDuration;
                    float realDuration = Duration * ScaleDuration;
                    amountPerDelta = (realDuration > 0f) ? (1f / realDuration) : 10000f;
                }

                return amountPerDelta * (isReversed ? -1 : 1);
            }
        }

        public float Factor
        {
            get { return factor; }
        }

        public Transform CachedTransform
        {
            get
            {
                if (cachedTransform == null)
                {
                    cachedTransform = transform;
                }

                return cachedTransform;
            }
        }

        public float Delay
        {
            set
            {
                delay = value;
                started = false;
            }
        }

        public virtual string TargetName
        {
            get { return gameObject.name; }
        }

        public bool NotUsedForScreenAnimatingCondition
        {
            get { return notUsedForScreenAnimatingCondition; }
            set { notUsedForScreenAnimatingCondition = value; }
        }

        #endregion

        #region Unity Lifecycle

        protected virtual void Awake()
        {
            if (ShouldSetBeginStateAtStart)
            {
                SetBeginStateImmediately();
            }

            enabled = false;
        }

        public virtual void Update()
        {
            float delta = IgnoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
            float time = IgnoreTimeScale ? Time.unscaledTime : Time.time;

            if (!started)
            {
                started = true;
                startTime = time + delay;
            }

            if (time < startTime)
            {
                return;
            }

            float oldFactor = factor;
            factor += AmountPerDelta * delta;

            if (timedCallbacks.Count > 0)
            {
                for (int i = 0; i < timedCallbacks.Count; i++)
                {
                    KeyValuePair<string, TimedCallback> pair = timedCallbacks[i];
                    TimedCallback timedCallback = pair.Value;
                    if (timedCallback.callback != null && oldFactor < timedCallback.time && factor > timedCallback.time)
                    {
                        timedCallback.callback(this);
                    }
                }
            }

            if (Style == Style.Loop)
            {
                if (factor > 1f)
                {
                    factor -= Mathf.Floor(factor);
                }
                else if (factor < 0f)
                {
                    factor = 1f - factor;
                }
            }
            else if (Style == Style.PingPong)
            {
                if (factor > 1f)
                {
                    factor = 1f - (factor - Mathf.Floor(factor));
                    amountPerDelta = -AmountPerDelta;
                }
                else if (factor < 0f)
                {
                    factor = -factor;
                    factor -= Mathf.Floor(factor);
                    amountPerDelta = -AmountPerDelta;
                }
            }

            if ((Style == Style.Once) && ((factor > 1f) || (factor < 0f)))
            {
                factor = Mathf.Clamp01(factor);

                Sample(factor, true);

                enabled = false;

                if (onFinished != null)
                {
                    onFinished(this);
                }

                InvokeWhenFinished.Call();
            }
            else
            {
                Sample(factor, false);
            }
        }

        #endregion

        #region Public methods

        public void Sample(float value, bool isFinished)
        {
            value = Mathf.Clamp01(value);

            switch (Method)
            {
                case Method.EaseIn:
                    value = 1f - Mathf.Sin(0.5f * Mathf.PI * (1f - value));

                    if (SteeperCurves)
                    {
                        value *= value;
                    }

                    break;

                case Method.EaseOut:
                    value = Mathf.Sin(0.5f * Mathf.PI * value);

                    if (SteeperCurves)
                    {
                        value = 1f - value;
                        value = 1f - value * value;
                    }

                    break;

                case Method.EaseInOut:
                    const float pi2 = Mathf.PI * 2f;
                    value = value - Mathf.Sin(value * pi2) / pi2;

                    if (SteeperCurves)
                    {
                        value = value * 2f - 1f;
                        float sign = (value < 0f) ? -1f : 1f;
                        value = 1f - sign * value;
                        value = 1f - value * value;
                        value = sign * value * 0.5f + 0.5f;
                    }

                    break;
            }

            if (Application.isPlaying)
            {
                TweenUpdateRuntime(
                    ((UseCurve && (AnimationCurve != null)) ? AnimationCurve.Evaluate(value) : value),
                    isFinished);
            }
            else
            {
                TweenUpdateEditor((UseCurve && (AnimationCurve != null)) ? AnimationCurve.Evaluate(value) : value);
            }
        }

        public void SwitchOnCurve()
        {
            UseCurve = true;
        }

        public void SwitchOffCurve()
        {
            UseCurve = false;
        }

        public void Play(bool forward)
        {
            IsReversed = !forward;
            enabled = true;
        }

        public bool IsReversed
        {
            get { return isReversed; }
            set { isReversed = value; }
        }

        public void Reverse()
        {
            isReversed = !isReversed;
        }

        public void ResetScaleDuration()
        {
            ScaleDuration = 1f;
        }

        public void ResetTween()
        {
            ResetTween(false);
        }

        protected void ResetTween(bool isFinished)
        {
            factor = IsReversed ? 1f : 0f;
            Sample(factor, isFinished);
        }

        public virtual bool IsBeginStateSet
        {
            get { return !enabled && factor < TweenFactorEpsilon; }
        }

        public virtual bool IsEndStateSet
        {
            get { return !enabled && factor > 1 - TweenFactorEpsilon; }
        }

        public void SetBeginStateDefault() //signature for animation event
        {
            SetBeginState();
        }

        public virtual void SetBeginState()
        {
            SetBeginState(0);
        }

        public virtual void SetBeginState(float delay)
        {
            SetBeginState(delay, null);
        }

        public virtual void SetBeginState(float delay, TweenCallback del)
        {
            if (!gameObject.activeSelf || !gameObject.activeInHierarchy)
            {
                SetBeginStateImmediately();
                if (del != null)
                {
                    del(this);
                }
            }
            else
            {
                Delay = delay;
                SetOnFinishedDelegate(del);
                Play(false);
            }
        }

        public void SetEndStateDefault() //signature for animation event
        {
            SetEndState();
        }

        public virtual void SetEndState(float delay = 0f, TweenCallback del = null)
        {
            if (!gameObject.activeSelf || !gameObject.activeInHierarchy)
            {
                SetEndStateImmediately();
                if (del != null)
                {
                    del(this);
                }
            }
            else
            {
                Delay = delay;

                SetOnFinishedDelegate(del);

                Play(true);
            }
        }

        public virtual void SetBeginStateImmediately()
        {
            factor = 0;
            Sample(0, true);
        }

        public virtual void SetEndStateImmediately()
        {
            factor = 1;
            Sample(1, true);
        }

        public void AddTimedCallback(string key, float time, TweenCallback call)
        {
            if (!string.IsNullOrEmpty(key))
            {
                AddTimedCallback(key, new TimedCallback(call, time));
            }
        }

        public void AddTimedCallback(string key, TimedCallback timedCallback)
        {
            if (string.IsNullOrEmpty(key) || timedCallback.callback == null)
            {
                return;
            }

            for (int i = 0; i < timedCallbacks.Count; i++)
            {
                var cb = timedCallbacks[i];
                if (cb.Key.Equals(key))
                {
                    return;
                }
            }

            timedCallbacks.Add(new KeyValuePair<string, TimedCallback>(key, timedCallback));
        }

        public void RemoveTimedCallback(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            for (int i = 0; i < timedCallbacks.Count; i++)
            {
                var cb = timedCallbacks[i];
                if (cb.Key.Equals(key))
                {
                    timedCallbacks.Remove(cb);
                    break;
                }
            }
        }

        public void ClearAllTimedCallbacks()
        {
            timedCallbacks.Clear();
        }

        // support old version!!!!!
        public void SetTimedCallback(float time, TweenCallback call)
        {
            const string oldVersionDefaultKey = "oldVersionDefaultKey";
            if (call != null)
            {
                for (int i = 0; i < timedCallbacks.Count; i++)
                {
                    var cb = timedCallbacks[i];
                    if (cb.Key.Equals(oldVersionDefaultKey))
                    {
                        timedCallbacks.Add(
                            new KeyValuePair<string, TimedCallback>(
                                oldVersionDefaultKey,
                                new TimedCallback(call, time)));
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < timedCallbacks.Count; i++)
                {
                    var cb = timedCallbacks[i];
                    if (cb.Key.Equals(oldVersionDefaultKey))
                    {
                        timedCallbacks.Remove(cb);
                        break;
                    }
                }
            }
        }

        public void AddOnFinishedDelegate(TweenCallback del)
        {
            if (del != null)
            {
                onFinished += del;
            }
        }

        public void RemoveOnFinishedDelegate(TweenCallback del)
        {
            if (del != null)
            {
                onFinished -= del;
            }
        }

        public void SetOnFinishedDelegate(TweenCallback del)
        {
            onFinished = del;
        }

        public bool IsRun
        {
            get { return enabled; }
        }

        public static T InitGo<T>(GameObject go)
            where T : Tweener
        {
            T tw = go.GetComponent<T>() ?? go.AddComponent<T>();
            tw.factor = 0f;
            return tw;
        }

        public static T InitGo<T>(GameObject go, float duration)
            where T : Tweener
        {
            T tw = go.GetComponent<T>() ?? go.AddComponent<T>();
            tw.Duration = duration;
            tw.factor = 0f;
            return tw;
        }

        #endregion

        #region Private methods

        protected abstract void TweenUpdateRuntime(float factor, bool isFinished);

        protected virtual void TweenUpdateEditor(float factor)
        {
            TweenUpdateRuntime(factor, false);
        }

        private IEnumerator WaitAndDo(float wait)
        {
            float start = Time.time;
            while ((Time.time - start) < wait)
            {
                yield return null;
            }

            if (waitAndDoCallback != null)
            {
                waitAndDoCallback();
            }
        }

        protected void RunWaitTimer(float waitTime, WaitAction todo)
        {
            waitAndDoCallback = todo;
            StartCoroutine(nameof(WaitAndDo), waitTime);
        }

        protected void ResetWaitTimer()
        {
            waitAndDoCallback = null;
            StopCoroutine(nameof(WaitAndDo));
        }

        #endregion
    }

    public enum Method
    {
        Linear,
        EaseIn,
        EaseOut,
        EaseInOut,
    }

    public enum Style
    {
        Once,
        Loop,
        PingPong,
    }

    public delegate void TweenCallback(ITweener tw);

    public delegate bool WaitAction();

    public interface ITweenerMini
    {
        bool IsBeginStateSet { get; }

        bool IsEndStateSet { get; }

        void SetBeginState(float delay = 0f, TweenCallback del = null);

        void SetEndState(float delay = 0f, TweenCallback del = null);

        void SetBeginStateImmediately();

        void SetEndStateImmediately();
    }

    public interface ITweener : ITweenerMini
    {
        void AddOnFinishedDelegate(TweenCallback del);

        void RemoveOnFinishedDelegate(TweenCallback del);

        void SetOnFinishedDelegate(TweenCallback del);

        string TargetName { get; }

        bool IsRun { get; }
    }

    public struct TimedCallback
    {
        public readonly TweenCallback callback;
        public readonly float time;

        public TimedCallback(TweenCallback callback, float time)
            : this()
        {
            this.callback = callback;
            this.time = time;
        }
    }
}