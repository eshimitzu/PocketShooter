using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityStandardAssets.CrossPlatformInput
{
    public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IPointerClickHandler
    {
        public enum AxisOption
        {
            // Options for which axes to use
            Both, // Use both
            OnlyHorizontal, // Only horizontal
            OnlyVertical // Only vertical
        }

        [SerializeField] private RectTransform thumb;
        [SerializeField] private RectTransform thumbRoot;
        [SerializeField] private float holdTimeout = .3f;

        public int MovementRange = 100;

        // The options for the axes that the still will use
        public AxisOption axesToUse = AxisOption.Both;

        // The name given to the horizontal axis for the cross platform input
        public string horizontalAxisName = "Horizontal";

        // The name given to the vertical axis for the cross platform input
        public string verticalAxisName = "Vertical";

        public UnityEvent onClick = new UnityEvent();
        public UnityEvent onDragStart = new UnityEvent();
        public UnityEvent onDragEnd = new UnityEvent();

        Vector2 m_StartPos;
        bool m_UseX; // Toggle for using the x axis
        bool m_UseY; // Toggle for using the Y axis

        // Reference to the joystick in the cross platform input
        CrossPlatformInputManager.VirtualAxis m_HorizontalVirtualAxis;

        // Reference to the joystick in the cross platform input
        CrossPlatformInputManager.VirtualAxis m_VerticalVirtualAxis;

        private float lastPressTime;
        private bool isDragging;
        private Vector3 initialThumbPosition;
        private Vector3 initialThumbRootPosition;
        private RectTransform screenRectTransform;

        void OnEnable()
        {
            CreateVirtualAxes();
        }

        void Start()
        {
            screenRectTransform = GetComponent<RectTransform>();
            m_StartPos = thumbRoot.localPosition;

            initialThumbPosition = thumb.localPosition;
            initialThumbRootPosition = thumbRoot.localPosition;
        }

        void UpdateVirtualAxes(Vector2 value)
        {
            var delta = value;
            delta /= MovementRange;
            if (m_UseX)
            {
                m_HorizontalVirtualAxis.Update(delta.x);
            }

            if (m_UseY)
            {
                m_VerticalVirtualAxis.Update(delta.y);
            }
        }

        void CreateVirtualAxes()
        {
            // set axes to use
            m_UseX = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyHorizontal);
            m_UseY = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyVertical);

            // create new axes based on axes to use
            if (m_UseX)
            {
                m_HorizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(horizontalAxisName);
                CrossPlatformInputManager.RegisterVirtualAxis(m_HorizontalVirtualAxis);
            }

            if (m_UseY)
            {
                m_VerticalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(verticalAxisName);
                CrossPlatformInputManager.RegisterVirtualAxis(m_VerticalVirtualAxis);
            }
        }

        public void OnDrag(PointerEventData data)
        {
            Vector2 position;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(screenRectTransform, data.position, data.pressEventCamera, out position))
            {
                var offset = position - m_StartPos;
                var magnitude = offset.magnitude;
                if (magnitude > MovementRange)
                {
                    offset = offset.normalized * MovementRange;
                    isDragging = true;
                }

                thumb.localPosition = new Vector3(offset.x, offset.y);
                UpdateVirtualAxes(thumb.localPosition);
            }
        }

        public void OnPointerUp(PointerEventData data)
        {
            thumbRoot.localPosition = initialThumbRootPosition;
            thumb.localPosition = initialThumbPosition;

            UpdateVirtualAxes(Vector2.zero);
            isDragging = false;
            onDragEnd.Invoke();
        }

        public void OnPointerDown(PointerEventData data)
        {
            Vector2 position;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(screenRectTransform, data.position, data.pressEventCamera, out position))
            {
                thumbRoot.localPosition = position;
                m_StartPos = position;
                lastPressTime = Time.time;
                onDragStart.Invoke();
            }
        }

        void OnDisable()
        {
            // remove the joysticks from the cross platform input
            if (m_UseX)
            {
                m_HorizontalVirtualAxis.Remove();
            }

            if (m_UseY)
            {
                m_VerticalVirtualAxis.Remove();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (Time.time - lastPressTime < holdTimeout && !isDragging)
            {
                onClick.Invoke();
            }
        }
    }
}