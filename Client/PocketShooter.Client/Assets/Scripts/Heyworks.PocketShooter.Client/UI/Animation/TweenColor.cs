using System;
using UnityEngine;

namespace Heyworks.PocketShooter.UI.Animation
{
    public interface ITweenColor
    {
        Color Color { get; set; }
    }

    [AddComponentMenu("HEYWORKS/Tween/Color")]
    public class TweenColor : Tweener
    {
        public Action OnColorChanged;

        public bool[] UseChanelMask = { true, true, true, true };
        public Color BeginColor = new Color(1f, 1f, 1f, 0f);
        public Color EndColor = new Color(1f, 1f, 1f, 1f);

        [SerializeField]
        private GameObject target;

        private SpriteRenderer spriteRenderer;
        private MeshRenderer meshRenderer;
        private ITweenColor objectColor;
        private bool needHandleMeshRenderer;

        protected override void Awake()
        {
            base.Awake();
            InitReference(false);
        }

        protected override void TweenUpdateRuntime(float factor, bool isFinished)
        {
            CurrentColor = Color.Lerp(BeginColor, EndColor, factor);
        }

        public override string TargetName => Target.name;

        public GameObject Target
        {
            get
            {
                if (target == null)
                {
                    target = gameObject;
                }

                return target;
            }

            set
            {
                target = value;
                if (target == null)
                {
                    target = Target;
                }

                InitReference(true);
            }
        }

        public bool IsReferenceSetUp
        {
            get
            {
                if (meshRenderer != null)
                {
                    return true;
                }

                if (objectColor != null)
                {
                    return true;
                }

                if (spriteRenderer != null)
                {
                    return true;
                }

                return false;
            }
        }

        public Color CurrentColor
        {
            get
            {
                InitReference(false);

                if (meshRenderer != null)
                {
                    return meshRenderer.sharedMaterial.color;
                }

                if (spriteRenderer != null)
                {
                    return spriteRenderer.sharedMaterial.color;
                }

                if (objectColor != null)
                {
                    return objectColor.Color;
                }

                return Color.black;
            }

            set
            {
                InitReference(false);
                if (meshRenderer != null)
                {
                    meshRenderer.sharedMaterial.color = ApplyChanelMask(value, meshRenderer.sharedMaterial.color);
                }
                else if (objectColor != null)
                {
                    objectColor.Color = ApplyChanelMask(value, objectColor.Color);
                }
                else if (spriteRenderer != null)
                {
                    spriteRenderer.sharedMaterial.color = ApplyChanelMask(value, spriteRenderer.sharedMaterial.color);
                }

                OnColorChanged?.Invoke();
            }
        }

        public bool IsOnlyAlphaTween => !UseChanelMask[0] && !UseChanelMask[1] && !UseChanelMask[2] && UseChanelMask[3];

        public static TweenColor SetColor(GameObject go, Color color, float duration = 1f)
        {
            var twc = InitGo<TweenColor>(go, duration);
            twc.InitReference(true);
            twc.BeginColor = twc.CurrentColor;
            twc.EndColor = color;
            twc.Play(true);
            return twc;
        }

        private void InitReference(bool force)
        {
            if (force || (spriteRenderer == null))
            {
                objectColor = Target.GetComponent<ITweenColor>();
                spriteRenderer = Target.GetComponent<SpriteRenderer>();
                if ((objectColor == null) &&
                    (spriteRenderer == null))
                {
                    meshRenderer = Target.GetComponent<MeshRenderer>();
                    if (meshRenderer != null && (meshRenderer.sharedMaterial == null ||
                                                 !meshRenderer.sharedMaterial.HasProperty("_Color")))
                    {
                        Debug.LogWarning("Wrong material!");
                        meshRenderer = null;
                    }
                }
            }
        }

        private Color ApplyChanelMask(Color value, Color source)
        {
            if (!UseChanelMask[0])
            {
                value.r = source.r;
            }

            if (!UseChanelMask[1])
            {
                value.g = source.g;
            }

            if (!UseChanelMask[2])
            {
                value.b = source.b;
            }

            if (!UseChanelMask[3])
            {
                value.a = source.a;
            }

            return value;
        }
    }
}