using System.Collections;
using UnityEngine;

public class BackgroundColorManager : MonoBehaviour
{
    public static BackgroundColorManager instance;

    private SpriteRenderer[] renderers;

    public float transitionTime = 2f;

    private void Awake()
    {
        instance = this;

        // Pobiera WSZYSTKIE SpriteRenderery z dzieci i wnuków
        renderers = GetComponentsInChildren<SpriteRenderer>();
    }

    public void ChangeColor(Color targetColor)
    {
        StopAllCoroutines();
        StartCoroutine(ChangeColorCoroutine(targetColor));
    }

    IEnumerator ChangeColorCoroutine(Color target)
    {
        Color[] startColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            startColors[i] = renderers[i].color;
        }

        float time = 0;

        while (time < transitionTime)
        {
            time += Time.deltaTime;

            float t = time / transitionTime;

            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].color =
                    Color.Lerp(startColors[i], target, t);
            }

            yield return null;
        }

        // finalny kolor
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].color = target;
        }
    }
}