using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    [Header("Referencje")]
    public PlayerHealth playerHealth; // odnośnik do gracza
    public Image fillImage;            // zielony prostokąt paska

    [Header("Ustawienia animacji")]
    public float smoothSpeed = 5f;     // szybkość płynnej animacji

    private float maxWidth;
    private float targetWidth;
    private float currentWidth;

    void Start()
    {
        maxWidth = fillImage.rectTransform.sizeDelta.x;
        currentWidth = maxWidth;
        targetWidth = maxWidth;

        // od razu ustaw pełny pasek po krótkim czasie (dla pewności, że gracz jest zainicjalizowany)
        Invoke(nameof(ForceFullHealthBar), 0.05f);
    }

    void Update()
    {
        if (playerHealth == null) return;

        float fillAmount = (float)playerHealth.currentHealth / playerHealth.maxHealth;
        fillAmount = Mathf.Clamp01(fillAmount);
        targetWidth = maxWidth * fillAmount;

        // płynne przesuwanie
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
