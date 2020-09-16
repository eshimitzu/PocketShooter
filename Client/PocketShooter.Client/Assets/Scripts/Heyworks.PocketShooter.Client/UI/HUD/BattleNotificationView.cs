using UnityEngine;
using UnityEngine.UI;

public class BattleNotificationView : MonoBehaviour
{
    [SerializeField]
    private Text notificationLabel;

    [SerializeField]
    private Image notificationBack;

    private float fadeSpeed = 1f;

    public void Setup(float fadeSpeed)
    {
        this.fadeSpeed = fadeSpeed;
    }

    private void Update()
    {
        UpdateAlpha(notificationLabel);
        UpdateAlpha(notificationBack);

        if (notificationLabel.color.a < 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void ShowMessage(string message)
    {
        notificationLabel.text = message;

        notificationLabel.color = new Color(notificationLabel.color.r, notificationLabel.color.g, notificationLabel.color.b, 1f);
        notificationBack.color = new Color(notificationBack.color.r, notificationBack.color.g, notificationBack.color.b, 1f);

        gameObject.SetActive(true);
    }

    private void UpdateAlpha(MaskableGraphic uiElement)
    {
        Color color = uiElement.color;
        color.a -= Time.deltaTime * fadeSpeed;
        uiElement.color = color;
    }
}
