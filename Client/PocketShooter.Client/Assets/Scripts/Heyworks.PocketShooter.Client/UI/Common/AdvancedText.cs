using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.Common
{
    [RequireComponent(typeof(Localize))]
    public class AdvancedText : Text
    {
        private Localize mLocalize;

        public override string text
        {
            get => base.text;
            set => mLocalize.Term = value;
        }

        protected override void Awake()
        {
            mLocalize = GetComponent<Localize>();
            base.Awake();
        }
    }
}