using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerLightByHealth : MonoBehaviour
{
    [Header("References")]
    public PlayerHealth playerHealth;
    public Light2D playerLight;

    [Header("Light Settings")]
    public float minLightIntensity = 0.2f;
    public float maxLightIntensity = 1f;
    public float smoothSpeed = 5f;

    private float targetIntensity;

    void Start()
    {
        if (playerLight != null)
            targetIntensity = playerLight.intensity;
    }

    void Update()
    {
        if (playerHealth == null || playerLight == null) return;

        float healthPercent = (float)playerHealth.currentHealth / playerHealth.maxHealth;

        // docelowa jasność zależna od HP
        targetIntensity = Mathf.Lerp(minLightIntensity, maxLightIntensity, healthPercent);

        // płynne przejście
        playerLight.intensity = Mathf.Lerp(
            playerLight.intensity,
            targetIntensity,
            Time.deltaTime * smoothSpeed
        );
    }
}
