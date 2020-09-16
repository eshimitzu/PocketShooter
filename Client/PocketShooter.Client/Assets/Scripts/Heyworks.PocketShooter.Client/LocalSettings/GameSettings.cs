using I2.Loc;
using UnityEngine;

namespace Heyworks.PocketShooter.LocalSettings
{
    public static class GameSettings
    {
        private static readonly string SoundOnKey = "SOUND_ON";
        private static readonly string MusicOnKey = "MUSIC_ON";
        private static readonly string LanguageKey = "Language";

        public static bool SoundOn
        {
            get => PlayerPrefsExtensions.GetPlayerPrefsBool(SoundOnKey, true);
            set
            {
                PlayerPrefsExtensions.SetPlayerPrefsBool(SoundOnKey, value);
                SoundOnToggle(value);
            }
        }

        public static bool MusicOn
        {
            get => PlayerPrefsExtensions.GetPlayerPrefsBool(MusicOnKey, true);
            set
            {
                PlayerPrefsExtensions.SetPlayerPrefsBool(MusicOnKey, value);
                MusicOnToggle(value);
            }
        }

        public static string CurrentLanguage
        {
            get => PlayerPrefs.GetString(LanguageKey, LocalizationManager.CurrentLanguage);
            set
            {
                PlayerPrefs.SetString(LanguageKey, value);
                SetLanguage(value);
            }
        }

        public static void Apply()
        {
            SoundOnToggle(SoundOn);
            MusicOnToggle(MusicOn);
            SetLanguage(CurrentLanguage);
        }

        private static void MusicOnToggle(bool isOn)
        {
            AkSoundEngine.MuteBackgroundMusic(!isOn);
        }

        private static void SoundOnToggle(bool isOn)
        {
            if (AkSoundEngine.IsInitialized())
            {
                if (isOn)
                {
                    AkSoundEngine.WakeupFromSuspend();
                }
                else
                {
                    AkSoundEngine.Suspend();
                }

                AkSoundEngine.RenderAudio();
            }
        }

        private static void SetLanguage(string language)
        {
            if (LocalizationManager.HasLanguage(language))
            {
                LocalizationManager.CurrentLanguage = language;
            }
        }
    }
}