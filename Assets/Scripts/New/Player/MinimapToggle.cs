using UnityEngine;
using System.Collections;

public class MinimapToggle : MonoBehaviour
{
    public GameObject minimapUI;
    public CanvasGroup canvasGroup;

    public float fadeDuration = 0.25f;

    private bool isOpen = false;
    private bool isAnimating = false;

    private void Start()
    {
        minimapUI.SetActive(false);

        canvasGroup.alpha = 0f;
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

        minimapUI.SetActive(true);

        float time = 0f;

        Time.timeScale = 0f;

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
        Time.timeScale = 1f;
        isAnimating = true;

        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;

            canvasGroup.alpha = Mathf.Lerp(1f, 0f, time / fadeDuration);

            yield return null;
        }

        canvasGroup.alpha = 0f;

        minimapUI.SetActive(false);

        isOpen = false;
        isAnimating = false;
    }
}