using I2.Loc;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI
{
    public static class LocalizedExtensions
    {
        public static void SetLocalizedText(this Text text, string value)
        {
            var str = LocalizationManager.GetTranslation(value, false);
            var localize = text.GetComponent<Localize>();
            if (localize)
            {
                localize.Term = value;
            }

            if (string.IsNullOrEmpty(str))
            {
                text.text = $"--{value}--";
            }
        }

        public static string Localized(this string text)
        {
            var str = LocalizationManager.GetTranslation(text, false);

            if (string.IsNullOrEmpty(str))
            {
                str = $"--{text}--";
            }

            return str;
        }
    }
}