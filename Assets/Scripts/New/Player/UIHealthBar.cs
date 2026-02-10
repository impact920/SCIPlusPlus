using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    [Header("References")]
    public PlayerHealth playerHealth; // Odnośnik do gracza
    public Image fillImage;            // Zielony prostokąt paska

    [Header("Smoothness")]
    public float smoothSpeed = 5f;     
    private float maxWidth;
    private float targetWidth;
    private float currentWidth;

    void Start()
    {
        maxWidth = fillImage.rectTransform.sizeDelta.x;
        currentWidth = maxWidth;
        targetWidth = maxWidth;

        // Ustaw pełny pasek po krótkim czasie (dla pewności, że gracz jest zainicjalizowany)
        Invoke(nameof(ForceFullHealthBar), 0.05f);
    }

    void Update()
    {
        if (playerHealth == null) return;

        float fillAmount = (float)playerHealth.currentHealth / playerHealth.maxHealth;
        fillAmount = Mathf.Clamp01(fillAmount);
        targetWidth = maxWidth * fillAmount;

        // Płynne przesuwanie
        currentWidth = Mathf.Lerp(currentWidth, targetWidth, Time.deltaTime * smoothSpeed);
        fillImage.rectTransform.sizeDelta = new Vector2(currentWidth, fillImage.rectTransform.sizeDelta.y);
    }

    void ForceFullHealthBar()
    {
        if (playerHealth == null) return;

        float fillAmount = (float)playerHealth.currentHealth / playerHealth.maxHealth;
        fillAmount = Mathf.Clamp01(fillAmount);

        currentWidth = targetWidth = maxWidth * fillAmount;
        fillImage.rectTransform.sizeDelta = new Vector2(currentWidth, fillImage.rectTransform.sizeDelta.y);
    }
}
