using UnityEngine;
using System.Collections;

public class MinimapToggle : MonoBehaviour
{
    public CanvasGroup canvasGroup;

    public float fadeDuration = 0.25f;

    private bool isOpen = false;
    private bool isAnimating = false;

    private void Start()
    {
        canvasGroup.alpha = 0f;

        // Ukrycie interakcji
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) && !isAnimating)
        {
            if (isOpen)
                StartCoroutine(FadeOut());
            else
                StartCoroutine(FadeIn());
        }
    }

    IEnumerator FadeIn()
    {
        isAnimating = true;

        Time.timeScale = 0f;

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;

            canvasGroup.alpha = Mathf.Lerp(0f, 1f, time / fadeDuration);

            yield return null;
        }

        canvasGroup.alpha = 1f;

        isOpen = true;
        isAnimating = false;
    }

    IEnumerator FadeOut()
    {
        isAnimating = true;

        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;

            canvasGroup.alpha = Mathf.Lerp(1f, 0f, time / fadeDuration);

            yield return null;
        }

        canvasGroup.alpha = 0f;

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        Time.timeScale = 1f;

        isOpen = false;
        isAnimating = false;
    }
}