using UnityEngine;

public class Switch : MonoBehaviour
{
    [Header("Obiekt do kontrolowania")]
    public GameObject targetObject; // Drzwi lub ściana, które mają być kontrolowane

    [Header("Stan przełącznika")]
    public bool isActivated = false; // Czy przełącznik jest aktywowany?
    public bool hasActivatedOnce = false; // Czy przełącznik został już użyty raz?

    [Header("Animacja lub ukrywanie")]
    public bool useAnimation = true; // Czy obiekt ma animację?
    public Animator targetAnimator; // Animator dla drzwi/ściany (opcjonalnie)
    public string animationTriggerName = "Open"; // Nazwa triggera animacji
    public float delayBeforeHide = 1.0f; // Opóźnienie przed ukryciem obiektu po animacji

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasActivatedOnce) // Gracz wchodzi w obszar przełącznika i animacja jeszcze nie była wykonana
        {
            ToggleSwitch();
        }
    }

    private void ToggleSwitch()
    {
        isActivated = !isActivated; // Zmiana stanu przełącznika
        hasActivatedOnce = true; // Oznaczenie, że animacja została wykonana

        if (useAnimation && targetAnimator != null)
        {
            // Aktywuj animację na obiekcie
            targetAnimator.SetTrigger(animationTriggerName);
            // Ukryj obiekt po opóźnieniu
            Invoke(nameof(HideTargetObject), delayBeforeHide);
        }
        else if (targetObject != null)
        {
            // Włącz lub wyłącz obiekt (np. drzwi/ściana)
            targetObject.SetActive(!isActivated);
        }

        Debug.Log("Przełącznik aktywowany: " + isActivated);
    }

    private void HideTargetObject()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(false); // Ukryj obiekt
        }
    }
}
