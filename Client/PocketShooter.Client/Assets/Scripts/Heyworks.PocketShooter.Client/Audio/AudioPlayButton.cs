using Heyworks.PocketShooter.Audio;
using UnityEngine;

namespace Heyworks.PocketShooter.AudioPlayButton
{
    /// <summary>
    /// Play button sounds on click.
    /// </summary>
    public class AudioPlayButton : MonoBehaviour
    {
        public void OnClick(string key)
        {
            AudioController.Instance.PostUIEvent(key);
        }
    }
}
