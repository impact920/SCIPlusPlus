using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PlayerLightByHealth : MonoBehaviour
{
    [Header("References")]
    public PlayerHealth playerHealth;
    public Light2D playerLight;
    public Image redVignette; // czerwony obraz UI na ekranie

    [Header("Light Settings")]
    public float minLightIntensity = 0.2f;
    public float maxLightIntensity = 1f;
    public float smoothSpeed = 5f;
    public int minHPThreshold = 10; // HP przy którym światło osiąga minimalną jasność

    [Header("Low HP Pulse")]
    [Range(0f, 1f)] public float lowHpThreshold = 0.3f;
    public float pulseSpeed = 4f;
    public float pulseStrength = 0.15f;

    [Header("Vignette Settings")]
    public float maxVignetteAlpha = 0.6f;
    public float vignetteSmooth = 5f;

    private float targetIntensity;
    private float vignetteTargetAlpha;

    void Update()
    {
        if (playerHealth == null || playerLight == null) return;

        float healthPercent = (float)playerHealth.currentHealth / playerHealth.maxHealth;

        UpdateLight(healthPercent);
        UpdateVignette(healthPercent);
    }

    void UpdateLight(float healthPercent)
    {
        // ograniczamy HP, aby minimalne światło osiągało się przy minHPThreshold
        float effectiveHP = Mathf.Max(playerHealth.currentHealth, minHPThreshold);
        float effectivePercent = effectiveHP / playerHealth.maxHealth;

        // podstawowa jasność zależna od "efektywnego" HP
        targetIntensity = Mathf.Lerp(minLightIntensity, maxLightIntensity, effectivePercent);

        // jeśli mało HP → pulsowanie
        if (healthPercent <= lowHpThreshold)
        {
            float pulse = Mathf.Sin(Time.time * pulseSpeed) * pulseStrength;
            targetIntensity += pulse;
        }

        // płynne przejście
        playerLight.intensity = Mathf.Lerp(
            playerLight.intensity,
            targetIntensity,
            Time.deltaTime * smoothSpeed
        );
    }

    void UpdateVignette(float healthPercent)
    {
        if (redVignette == null) return;

        // im mniej HP, tym mocniejsza czerwień
        vignetteTargetAlpha = Mathf.Lerp(maxVignetteAlpha, 0f, healthPercent);

        Color c = redVignette.color;
        c.a = Mathf.Lerp(c.a, vignetteTargetAlpha, Time.deltaTime * vignetteSmooth);
        redVignette.color = c;
    }
}
