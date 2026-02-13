using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    [Header("Damage → Shake mapping")]
    public float minDamage = 5f;
    public float maxDamage = 50f;

    public float minStrength = 0.05f;
    public float maxStrength = 0.35f;

    public float minDuration = 0.05f;
    public float maxDuration = 0.2f;

    private Coroutine shakeCoroutine;

    private void Awake()
    {
        Instance = this;
    }

    public void ShakeFromDamage(float damage)
    {
        // Normalizacja 0–1
        float t = Mathf.InverseLerp(minDamage, maxDamage, damage);

        float strength = Mathf.Lerp(minStrength, maxStrength, t);
        float duration = Mathf.Lerp(minDuration, maxDuration, t);

        Shake(strength, duration);
    }

    public void Shake(float strength, float duration)
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(ShakeRoutine(strength, duration));
    }

    private IEnumerator ShakeRoutine(float strength, float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            Vector3 basePos = transform.localPosition;

            float offsetX = Random.Range(-1f, 1f) * strength;
            float offsetY = Random.Range(-1f, 1f) * strength;

            transform.localPosition = basePos + new Vector3(offsetX, offsetY, 0f);

            yield return null;
        }
    }
}
