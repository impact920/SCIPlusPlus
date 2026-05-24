using UnityEngine;
using System.Collections;

public class ShowTextOnEnter : MonoBehaviour
{
    public GameObject textObject;
    public float fadeDuration = 1f;

    [Header("Czas wyświetlania (0 = dopóki gracz stoi)")]
    public float displayTime = 0f;

    [Header("Opcjonalny limit")]
    public int maxDisplays = 0;

    [Header("Zapis (opcjonalny)")]
    public string triggerID;

    private int currentDisplays = 0;
    private CanvasGroup canvasGroup;
    private Coroutine hideCoroutine;

    void Start()
    {
        // NULL CHECK
        if (textObject == null)
        {
            Debug.LogWarning("TextObject nie jest przypisany!");
            return;
        }

        textObject.SetActive(true);

        canvasGroup = textObject.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = textObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;

        if (!string.IsNullOrEmpty(triggerID))
        {
            currentDisplays = PlayerPrefs.GetInt(triggerID, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        // NULL CHECK
        if (canvasGroup == null || textObject == null) return;

        if (maxDisplays > 0 && currentDisplays >= maxDisplays) return;

        currentDisplays++;

        if (!string.IsNullOrEmpty(triggerID))
        {
            PlayerPrefs.SetInt(triggerID, currentDisplays);
            PlayerPrefs.Save();
        }

        StopAllCoroutines();
        StartCoroutine(FadeIn());

        if (displayTime > 0f)
        {
            if (hideCoroutine != null)
                StopCoroutine(hideCoroutine);

            hideCoroutine = StartCoroutine(HideAfterTime());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        // NULL CHECK
        if (canvasGroup == null || textObject == null) return;

        if (displayTime > 0f) return;

        if (canvasGroup.alpha <= 0f) return;

        StopAllCoroutines();
        StartCoroutine(FadeOut());
    }

    IEnumerator HideAfterTime()
    {
        yield return new WaitForSeconds(displayTime);

        // NULL CHECK
        if (canvasGroup != null)
            StartCoroutine(FadeOut());
    }

    IEnumerator FadeIn()
    {
        // NULL CHECK
        if (canvasGroup == null) yield break;

        float time = 0f;

        while (time < fadeDuration)
        {
            // jeśli obiekt został usunięty w trakcie
            if (canvasGroup == null)
                yield break;

            time += Time.deltaTime;
            canvasGroup.alpha = time / fadeDuration;
            yield return null;
        }

        if (canvasGroup != null)
            canvasGroup.alpha = 1f;
    }

    IEnumerator FadeOut()
    {
        // NULL CHECK
        if (canvasGroup == null) yield break;

        float time = 0f;

        while (time < fadeDuration)
        {
            if (canvasGroup == null)
                yield break;

            time += Time.deltaTime;
            canvasGroup.alpha = 1f - (time / fadeDuration);
            yield return null;
        }

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;
    }
}