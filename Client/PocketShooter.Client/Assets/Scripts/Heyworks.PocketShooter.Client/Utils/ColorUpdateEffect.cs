using System.Collections;
using UnityEngine;

namespace Heyworks.PocketShooter.Utils
{
    public class ColorUpdateEffect : MonoBehaviour
    {
        [SerializeField]
        private GameObject target;

        [SerializeField]
        private Color defaultColor;

        [SerializeField]
        private Color updateColor;

        public bool IsPlaying { get; private set; }

        public void Play()
        {
            if (!IsPlaying)
            {
                var hashtable = new Hashtable
                {
                    { iT.ColorTo.time, 0.5f },
                    { iT.ColorTo.color, updateColor },
                    { iT.ColorTo.looptype, iTween.LoopType.pingPong },
                    { iT.ColorTo.easetype, iTween.EaseType.linear },
                };

                iTween.ColorTo(target, hashtable);
                IsPlaying = true;
            }
        }

        public void Stop()
        {
            if (IsPlaying)
            {
                IsPlaying = false;
                iTween.Stop(target);
                iTween.ColorUpdate(target, defaultColor, 0);
            }
        }
    }
}