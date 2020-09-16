using UnityEngine;

namespace Heyworks.PocketShooter.Configuration
{
    [CreateAssetMenu(fileName = "DeepLinksConfig", menuName = "Heyworks/Settings/Create Deep Links Config")]
    public class DeepLinksConfig : ScriptableObject
    {
        [SerializeField]
        private string pocketShooterIos;

        [SerializeField]
        private string pocketShooterAndroid;

        public string PocketShooter
        {
            get
            {
                #if UNITY_ANDROID
                    return pocketShooterAndroid;
                #elif UNITY_IOS
                    return pocketShooterIos;
                #endif

                return "https://perevod.mtbank.by/?gclid=EAIaIQobChMIopq19qbu4wIVy5QYCh2AgQHIEAAYASABEgLXAfD_BwE";
            }
        }
    }
}