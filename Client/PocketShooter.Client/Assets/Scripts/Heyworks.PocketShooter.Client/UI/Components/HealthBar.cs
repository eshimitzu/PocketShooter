using Heyworks.PocketShooter.UI.Lobby;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.Components
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField]
        private RectTransform rectTransform;

        [SerializeField]
        private UIProgressBar progressBar;

        [SerializeField]
        private GameObject eyeLashPrefab;

        public void SetColor(Color color)
        {
            progressBar.GetComponent<Image>().color = color;
        }

        public void UpdateProgress(float health, float maxHealth)
        {
            progressBar.Progress = health / maxHealth;
        }

        public void SetEyeLashesForHP(float maxHP, int countHPForOneEyeLash, float width, float height)
        {
            int countEyeLashes = (int)(maxHP / countHPForOneEyeLash);

            float eyeLashsize = countHPForOneEyeLash / maxHP * rectTransform.rect.size.x;

            for (int i = 0; i < countEyeLashes; i++)
            {
                GameObject eyeLash = Instantiate(eyeLashPrefab);
                eyeLash.transform.SetParent(rectTransform);

                RectTransform eyeLashRectTransform = eyeLash.GetComponent<RectTransform>();
                eyeLashRectTransform.localScale = Vector3.one;
                eyeLashRectTransform.sizeDelta = new Vector2(width, height);
                eyeLashRectTransform.anchoredPosition = new Vector2(eyeLashsize * (i + 1) - width, 0f);
            }
        }
    }
}