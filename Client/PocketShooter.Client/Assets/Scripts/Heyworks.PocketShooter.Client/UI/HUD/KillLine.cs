using Heyworks.PocketShooter.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.HUD
{
    public class KillLine : MonoBehaviour
    {
        [SerializeField]
        private Text killerLabel;

        [SerializeField]
        private Text deadLabel;

        [SerializeField]
        private GameVisualSettings gameVisualSettings;

        [SerializeField]
        private Image weaponIcon;

        private float disappearTime;
        private float currentDisappearTime;

        public bool IsDisappearing { get; private set; }

        public void Setup(string killer, string victim, bool isKillerTeamMy, bool isVictimTeamMy, bool isLocalPlayerKiller, bool isLocalPlayerVictim)
        {
            killerLabel.text = killer;
            deadLabel.text = victim;

            killerLabel.color = isLocalPlayerKiller ? gameVisualSettings.PlayerColor : (isKillerTeamMy ? gameVisualSettings.FriendColor : gameVisualSettings.EnemyColor);
            deadLabel.color = isLocalPlayerVictim ? gameVisualSettings.PlayerColor : (isVictimTeamMy ? gameVisualSettings.FriendColor : gameVisualSettings.EnemyColor);
        }

        public void StartDisappearing(float disappearTime)
        {
            IsDisappearing = true;
            this.disappearTime = disappearTime;
            currentDisappearTime = 0f;
        }

        private void Update()
        {
            if (IsDisappearing)
            {
                if (currentDisappearTime > disappearTime)
                {
                    Destroy(gameObject);
                }

                currentDisappearTime += Time.deltaTime;

                killerLabel.color = new Color(killerLabel.color.r, killerLabel.color.g, killerLabel.color.b, Mathf.Lerp(1f, 0f, currentDisappearTime / disappearTime));
                deadLabel.color = new Color(deadLabel.color.r, deadLabel.color.g, deadLabel.color.b, Mathf.Lerp(1f, 0f, currentDisappearTime / disappearTime));
                weaponIcon.color = new Color(weaponIcon.color.r, weaponIcon.color.g, weaponIcon.color.b, Mathf.Lerp(1f, 0f, currentDisappearTime / disappearTime));
            }
        }
    }
}