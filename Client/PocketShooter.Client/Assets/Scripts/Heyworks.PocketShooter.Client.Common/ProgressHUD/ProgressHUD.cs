using System;
using Heyworks.PocketShooter.Singleton;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents class for progress HUD.
/// </summary>
public sealed class ProgressHUD : Singleton<ProgressHUD>
{
    #region [Private fields]

    [SerializeField]
    private GameObject view = null;

    [SerializeField]
    private Image[] bullets = null;

    [SerializeField]
    private float fadeTime = 0.145f;

    [SerializeField]
    private float defaultViewAppearDelay = 0.7f;

    [SerializeField]
    private CanvasGroup canvasGroup;

    private bool isVisible;

    #endregion

    #region [Properties]

    /// <summary>
    /// Gets default time the view waits before appearing after show invocation.
    /// </summary>
    public float DefaultViewAppearingDelay
    {
        get
        {
            return defaultViewAppearDelay;
        }
    }

    #endregion

    #region [Unity events]

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        //SceneManager.sceneLoaded += SceneManager_SceneLoaded;
        view.SetActive(false);
    }

    #endregion

    #region [Public methods]

    /// <summary>
    /// Shows progress HUD.
    /// </summary>
    [ContextMenu("Show")]
    public void Show()
    {
        Show(defaultViewAppearDelay);
    }

    /// <summary>
    /// Hides progress HUD.
    /// </summary>
    [ContextMenu("Hide")]
    public void Hide()
    {
        if (isVisible)
        {
            //GraphicsLog.Log.LogTrace("Hide progress HUD.");

            HideView();
        }
    }

    /// <summary>
    /// Show progress HUD with delay.
    /// </summary>
    /// <param name="delay"> Delay in seconds before showing progress HUD. </param>
    public void Show(float delay)
    {
        if (!isVisible)
        {
            //GraphicsLog.Log.LogTrace("Show progress HUD.");

            ShowView(delay);
        }
    }

    #endregion

    #region [View methods]

    private void ShowView(float delay)
    {
        isVisible = true;
        StartCoroutine(ProgressCoroutine(delay));
    }

    private void HideView()
    {
        isVisible = false;
        StopAllCoroutines();
        view.SetActive(false);
    }

    private IEnumerator ProgressCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        view.SetActive(true);

        canvasGroup.alpha = 0f;
        StartCoroutine(FadeCanvasGroupIn());

        foreach (var bullet in bullets)
        {
            bullet.color = new Color(1, 1, 1, 0);
        }

        var fadeIn = true;

        while (true)
        {
            foreach (var bullet in bullets)
            {

                CrossFadeAlpha(bullet, fadeIn, fadeTime);
                yield return new WaitForSeconds(fadeTime);
            }

            fadeIn = !fadeIn;
        }
    }

    private static void CrossFadeAlpha(Graphic img, bool fadeIn, float duration)
    {
        // This is workaround fix of strange CrossFadeAlpha behaviour
        if (fadeIn)
        {
            //Make the alpha 1
            Color fixedColor = img.color;
            fixedColor.a = 1f;
            img.color = fixedColor;

            //Set the 0 to zero then duration to 0
            img.CrossFadeAlpha(0f, 0f, true);
        }

        //Finally perform CrossFadeAlpha
        img.CrossFadeAlpha(fadeIn ? 1f : 0f, duration, true);
    }


    private IEnumerator FadeCanvasGroupIn()
    {
        float start = Time.time;
        while (Math.Abs(canvasGroup.alpha - 1) > 0.01)
        {
            float elapsed = Time.time - start;
            float normalizedTime = Mathf.Clamp(elapsed / fadeTime, 0, 1);
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 1, normalizedTime);
            yield return 0;
        }
        yield return true;
    }

    #endregion
}
