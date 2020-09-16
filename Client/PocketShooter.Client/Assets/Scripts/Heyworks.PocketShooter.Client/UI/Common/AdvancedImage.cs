using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.Common
{
    [RequireComponent(typeof(Image))]
    public class AdvancedImage : BaseMeshEffect
    {
        private enum GradientType
        {
            None = 0,
            Horizontal = 1,
            Vertical = 2,
        }

        [SerializeField]
        private float skewX = 0.065f;

        [SerializeField]
        private float skewY;

        [SerializeField]
        private RectTransform skewAnchor;

        [SerializeField]
        private GradientType gradientType;

        [SerializeField]
        private Gradient gradient;

        private UIVertex vertex;

        public Gradient Gradient
        {
            get => gradient;
            set
            {
                gradient = value;
                if (this.graphic != null)
                {
                    this.graphic.SetVerticesDirty();
                }
            }
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            if (vh.currentVertCount == 0)
            {
                return;
            }

            vh.PopulateUIVertex(ref vertex, 0);
            Vector2 min = vertex.position;
            Vector2 max = vertex.position;

            for (var i = 0; i < vh.currentVertCount; i++)
            {
                vh.PopulateUIVertex(ref vertex, i);
                Vector3 pos = vertex.position;

                min.x = min.x > pos.x ? pos.x : min.x;
                min.y = min.y > pos.y ? pos.y : min.y;

                max.x = max.x < pos.x ? pos.x : max.x;
                max.y = max.y < pos.y ? pos.y : max.y;
            }

            Vector2 size = max - min;

            if (gradientType != GradientType.None)
            {
                for (var i = 0; i < vh.currentVertCount; i++)
                {
                    vh.PopulateUIVertex(ref vertex, i);
                    Vector3 pos = vertex.position;

                    float time = 0;
                    switch (gradientType)
                    {
                        case GradientType.Horizontal:
                            time = (pos.x - min.x) / size.x;
                            break;
                        case GradientType.Vertical:
                            time = (pos.y - min.y) / size.y;
                            break;
                    }

                    vertex.color = Gradient.Evaluate(time);
                    vh.SetUIVertex(vertex, i);
                }
            }

            Vector3 center = (max + min) * 0.5f;
            if (skewAnchor)
            {
                var rectTransform = GetComponent<RectTransform>();
                Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(null, skewAnchor.position);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPosition, null, out Vector2 localPoint);
                center = localPoint;
            }

            if (skewX > 0 || skewY > 0)
            {
                Matrix4x4 skew = Matrix4x4.identity;
                skew[0, 1] = skewX;
                skew[1, 0] = skewY;

                for (var i = 0; i < vh.currentVertCount; i++)
                {
                    vh.PopulateUIVertex(ref vertex, i);
                    Vector3 skewPoint = skew.MultiplyPoint(vertex.position - center);
                    vertex.position = center + skewPoint;
                    vh.SetUIVertex(vertex, i);
                }
            }
        }
    }
}