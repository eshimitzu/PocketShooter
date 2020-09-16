using System;
using System.Diagnostics.CodeAnalysis;
using Heyworks.PocketShooter.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

namespace Heyworks.PocketShooter.UI.Common
{
    /// <summary>
    /// Represents TouchPad.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class TouchPad : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
    {
        private const float ReferenceDpi = 200f;
        private const int StationaryFramesToReset = 5;

        /// <summary>
        /// The name given to the horizontal axis for the cross platform input.
        /// </summary>
        [SerializeField]
        private string horizontalAxisName = "Horizontal";

        /// <summary>
        /// The name given to the vertical axis for the cross platform input.
        /// </summary>
        [SerializeField]
        private string verticalAxisName = "Vertical";
        [SerializeField]
        private float sensitivityX = 0.35f;
        [SerializeField]
        private float sensitivityY = 0.35f;
        [SerializeField]
        private Rect rect = new Rect(0.5f, 0.0f, 0.5f, 0.7f);
        [SerializeField]
        private float smoothnessTime = 0.1f;

        private CrossPlatformInputManager.VirtualAxis horizontalVirtualAxis; // Reference to the joystick in the cross platform input
        private CrossPlatformInputManager.VirtualAxis verticalVirtualAxis; // Reference to the joystick in the cross platform input

        private float doubleClickInterval = 0.4f;
        private float lastClick;
        private bool hasPointInArea;
        private float inputFactor;
        private float lastStationaryTime;
        private Interpolate.Function interpolate = Interpolate.Ease(Interpolate.EaseType.EaseInQuad);

#if UNITY_EDITOR
        private Vector3 prevPosition;
#else
        private Vector2 prevPosition;
#endif

        private int stationaryFrames = 0;

        /// <summary>
        /// Action on DoubleClick.
        /// </summary>
        public event Action DoubleClick;

        /// <summary>
        /// Gets if you multiply this value with any other pixel delta (e.g. ScreenDelta), then it will become device resolution independent.
        /// </summary>
        private static float ScalingFactor
        {
            get
            {
                // Get the current screen DPI
                var dpi = UnityEngine.Screen.dpi;

                // If it's 0 or less, it's invalid, so return the default scale of 1.0
                if (dpi <= 0)
                {
                    return 1.0f;
                }

                // DPI seems valid, so scale it against the reference DPI
                return Mathf.Sqrt(ReferenceDpi) / Mathf.Sqrt(dpi);
            }
        }

        private void OnEnable()
        {
            horizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(horizontalAxisName);
            CrossPlatformInputManager.RegisterVirtualAxis(horizontalVirtualAxis);
            verticalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(verticalAxisName);
            CrossPlatformInputManager.RegisterVirtualAxis(verticalVirtualAxis);
        }

        private void OnDisable()
        {
            if (CrossPlatformInputManager.AxisExists(horizontalAxisName))
            {
                CrossPlatformInputManager.UnRegisterVirtualAxis(horizontalVirtualAxis);
            }

            if (CrossPlatformInputManager.AxisExists(verticalAxisName))
            {
                CrossPlatformInputManager.UnRegisterVirtualAxis(verticalVirtualAxis);
            }
        }

        private void Update()
        {
#if UNITY_EDITOR
            ProcessMouseInput();
#else
            ProcessTouchInput();
#endif
        }

        /// <summary>
        /// OnPointerUp.
        /// </summary>
        /// <param name="eventData">PointerEventData.</param>
        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed.")]
        public void OnPointerUp(PointerEventData eventData)
        {
            if ((lastClick + doubleClickInterval) > Time.time)
            {
                DoubleClick?.Invoke();
            }
            else if (!eventData.IsPointerMoving())
            {
                lastClick = Time.time;
            }
        }

        /// <summary>
        /// OnPointerDown.
        /// </summary>
        /// <param name="eventData">PointerEventData.</param>
        public void OnPointerDown(PointerEventData eventData)
        {
        }

#if UNITY_EDITOR
        private void ProcessMouseInput()
        {
            if (Input.GetMouseButton(0))
            {
                bool first = !hasPointInArea;
                if (!hasPointInArea)
                {
                    hasPointInArea = true;
                    prevPosition = Input.mousePosition;
                }

                Vector2 delta = (Input.mousePosition - prevPosition) * ScalingFactor;
                var smoothDelta = SmoothInput(delta, first);

                prevPosition = Input.mousePosition;

                // Debug.Log($"Delta: {delta}, smoothDelta: {smoothDelta}");
                UpdateVirtualAxes(smoothDelta);
            }
            else
            {
                hasPointInArea = false;
                UpdateVirtualAxes(Vector2.zero);
            }
        }
#else
        private void ProcessTouchInput()
        {
            var index = GetActiveTouchIndex();

            if (index >= 0)
            {
                var touch = Input.GetTouch(index);

                bool first = !hasPointInArea;
                if (!hasPointInArea)
                {
                    hasPointInArea = true;
                    prevPosition = touch.position;
                }

                var delta = touch.phase == TouchPhase.Moved ? (touch.position - prevPosition) * ScalingFactor : Vector2.zero;
                var smoothDelta = SmoothInput(delta, first);

                prevPosition = touch.position;

                // Debug.Log($"Delta: {delta}, phase: {touch.phase}, smoothDelta: {smoothDelta}");
                UpdateVirtualAxes(smoothDelta);
            }
            else
            {
                hasPointInArea = false;
                UpdateVirtualAxes(Vector2.zero);
            }
        }

#endif

        private void UpdateVirtualAxes(Vector2 value)
        {
            value.x *= sensitivityX;
            value.y *= sensitivityY;

            horizontalVirtualAxis.Update(value.x);
            verticalVirtualAxis.Update(value.y);
        }

        private Vector2 SmoothInput(Vector2 value, bool first)
        {
            stationaryFrames = (value.magnitude > 0) ? 0 : (stationaryFrames + 1);

            if (first || stationaryFrames >= StationaryFramesToReset)
            {
                inputFactor = 0;
                lastStationaryTime = Time.time;
            }

            inputFactor = interpolate(0.0f, 1.0f, Time.time - lastStationaryTime, smoothnessTime);

            return inputFactor * value;
        }

        private int GetActiveTouchIndex()
        {
            for (int index = 0; index < Input.touchCount; index++)
            {
                var touch = Input.GetTouch(index);

                if (IsPositionInRect(touch.position))
                {
                    return index;
                }
            }

            return -1;
        }

        private bool IsPositionInRect(Vector2 position)
        {
            var w = UnityEngine.Screen.width;
            var h = UnityEngine.Screen.height;
            var normPosition = Vector2.Scale(position, new Vector3(1f / w, 1f / h));

            return rect.Contains(normPosition);
        }
    }
}