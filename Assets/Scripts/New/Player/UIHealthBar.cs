using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHealthBar : MonoBehaviour
{
    [Header("References")]
    public PlayerHealth playerHealth; // Odnośnik do gracza
    public Slider healthSlider;       // Slider zamiast Image
    public TMP_Text healthText;       // Tekst wyświetlający życie (liczbowo)

    [Header("Smoothness")]
    public float smoothSpeed = 5f;     
    private float targetValue;
    private float currentValue;

    void Start()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = playerHealth.maxHealth;
            healthSlider.value = playerHealth.currentHealth;
            currentValue = playerHealth.currentHealth;
            targetValue = playerHealth.currentHealth;
        }

        // Ustaw pełny pasek po krótkim czasie
        Invoke(nameof(ForceFullHealthBar), 0.05f);
    }

    void Update()
    {
        if (playerHealth == null || healthSlider == null) return;

        // Celowa wartość slidera
        targetValue = playerHealth.currentHealth;

        // Płynne przesuwanie slidera
        currentValue = Mathf.Lerp(currentValue, targetValue, Time.deltaTime * smoothSpeed);
        healthSlider.value = currentValue;

        // Aktualizacja tekstu życia
        if (healthText != null)
        {
            healthText.text = playerHealth.currentHealth.ToString();
        }
    }

    void ForceFullHealthBar()
    {
        if (playerHealth == null || healthSlider == null) return;

        currentValue = targetValue = playerHealth.currentHealth;
        healthSlider.value = currentValue;

        if (healthText != null)
        {
            healthText.text = playerHealth.currentHealth.ToString();
        }
    }
}