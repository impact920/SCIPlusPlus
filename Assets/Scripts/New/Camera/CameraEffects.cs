using UnityEngine;
using System.Collections;

public class CameraEffects : MonoBehaviour
{
    public static CameraEffects Instance;

    private Camera cam;

    private Coroutine zoomCoroutine;

    private void Awake()
    {
        Instance = this;
        cam = GetComponent<Camera>();
    }

    // ===== STATIC CALL =====

    public static void Zoom(float targetSize, float duration)
    {
        if (Instance != null)
        {
            Instance.StartZoom(targetSize, duration);
        }
    }

    // ===== ZOOM =====

    private void StartZoom(float targetSize, float duration)
    {
        if (zoomCoroutine != null)
            StopCoroutine(zoomCoroutine);

        zoomCoroutine = StartCoroutine(ZoomRoutine(targetSize, duration));
    }

    private IEnumerator ZoomRoutine(float targetSize, float duration)
    {
        float startSize = cam.orthographicSize;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            cam.orthographicSize = Mathf.Lerp(
                startSize,
                targetSize,
                timer / duration
            );

            yield return null;
        }

        cam.orthographicSize = targetSize;
    }
}